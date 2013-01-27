
namespace SV.UPnPLite.Protocols.SSDP
{
    using SV.UPnPLite.Extensions;
    using SV.UPnPLite.Protocols.SSDP.Messages;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text;
    using Windows.Networking;
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;

    /// <summary>
    ///     A Win RT implementation of the SSDP protocol.
    /// </summary>
    internal class SSDPServer : ISSDPServer
    {
        #region Constants

        private const string RFC1123DateFormat = "ddd, dd MMM yyyy hh:mm:ss EST";

        private const string MulticastAddress = "239.255.255.250";

        private const int MulticastPort = 1900;

        private const string MSearchRequestFormattedString =
            "M-SEARCH * HTTP/1.1" + "\r\n" +
            "HOST: 239.255.255.250:1900" + "\r\n" +
            "MAN: \"ssdp:discover\"" + "\r\n" +
            "ST: {0}" + "\r\n" +
            "MX: {1}" + "\r\n" +
            "Content-Length: 0" + "\r\n\r\n";

        #endregion

        #region Fields

        private static ISSDPServer instance;

        private static object instanceSyncObject = new object();

        private readonly Subject<NotifyMessage> notifyMessages = new Subject<NotifyMessage>();

        private readonly DatagramSocket server;

        private readonly HostName multicastHost;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SSDPServer" /> class.
        /// </summary>
        private SSDPServer()
        {
            this.multicastHost = new HostName(MulticastAddress);

            this.server = new DatagramSocket();
            this.server.MessageReceived += this.NotifyMessageReceived;
            this.server.BindEndpointAsync(null, MulticastPort.ToString()).GetAwaiter().GetResult();
            this.server.JoinMulticastGroup(this.multicastHost);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     An observable collection which contains notifications from devices.
        /// </summary>
        public IObservable<NotifyMessage> NotifyMessages { get { return this.notifyMessages; } }

        public static ISSDPServer Instance
        {
            get
            {
                lock (instanceSyncObject)
                {
                    if (instance == null)
                    {
                        instance = new SSDPServer();
                    }
                }

                return instance;
            }

        }

        #endregion

        #region Members

        /// <summary>
        ///     Searches for an available devices of specified type.
        /// </summary>
        /// <param name="searchTarget">
        ///     The type of the devices to search for.
        /// </param>
        /// <param name="timeForResponse">
        ///     The time (in seconds) of a search.
        /// </param>
        /// <returns>
        ///     An observable collection which contains search results.
        /// </returns>
        public IObservable<SearchResponseMessage> Search(string searchTarget, int timeForResponse)
        {
            return Observable.Create<SearchResponseMessage>(
                observer =>
                {
                    var searchSocket = new DatagramSocket();

                    // Handling responses from found devices
                    searchSocket.MessageReceived += async (sender, args) =>
                        {
                            var dataReader = args.GetDataReader();
                            dataReader.InputStreamOptions = InputStreamOptions.Partial;

                            if (dataReader.UnconsumedBufferLength == 0)
                            {
                                await dataReader.LoadAsync(1024);
                            }

                            var message = dataReader.ReadString(dataReader.UnconsumedBufferLength);

                            var response = ParseSearchResponseMessage(message);
                            if (response != null)
                            {
                                observer.OnNext(response);
                            }
                        };

                    searchSocket.BindEndpointAsync(null, "0").GetAwaiter().GetResult();
                    searchSocket.JoinMulticastGroup(this.multicastHost);

                    // Sending the search request to a multicast group
                    var outputStream = searchSocket.GetOutputStreamAsync(this.multicastHost, MulticastPort.ToString()).GetAwaiter().GetResult();
                    var request = MSearchRequestFormattedString.F(searchTarget, timeForResponse);
                    var buffer = Encoding.UTF8.GetBytes(request).AsBuffer();
                    outputStream.WriteAsync(buffer);
                    outputStream.WriteAsync(buffer);

                    // Stop listening for a devices when timeout for responses is expired
                    Observable.Timer(TimeSpan.FromSeconds(timeForResponse)).Subscribe(
                        s =>
                        {
                            observer.OnCompleted();
                            searchSocket.Dispose();
                        });

                    return searchSocket.Dispose;
                });
        }

        private async void NotifyMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            var dataReader = args.GetDataReader();
            dataReader.InputStreamOptions = InputStreamOptions.Partial;

            if (dataReader.UnconsumedBufferLength == 0)
            {
                await dataReader.LoadAsync(1024);
            }

            var message = dataReader.ReadString(dataReader.UnconsumedBufferLength);
            var lines = message.SplitIntoLines();
            if (lines.Any())
            {
                var statusString = lines[0].ToUpper();
                var headers = ParseHeaders(lines.Skip(1));

                if (statusString.StartsWith("NOTIFY"))
                {
                    try
                    {
                        var subType = SafeRead<NotifyMessageType?>(headers, "NTS");
                        if (subType.HasValue)
                        {
                            var response = new NotifyMessage
                            {
                                BootId = SafeRead<int>(headers, "BOOTID.UPNP.ORG"),
                                NextBootId = SafeRead<int>(headers, "NEXTBOOTID.UPNP.ORG"),
                                ConfigId = SafeRead<int>(headers, "CONFIGID.UPNP.ORG"),
                                Host = SafeRead<string>(headers, "HOST"),
                                Location = SafeRead<string>(headers, "LOCATION"),
                                MaxAge = ParseMaxAge(SafeRead<string>(headers, "CACHE-CONTROL")),
                                NotificationSubtype = subType.Value,
                                NotificationType = SafeRead<string>(headers, "NT"),
                                SearchPort = SafeRead<int>(headers, "SEARCHPORT.UPNP.ORG"),
                                Server = SafeRead<string>(headers, "SERVER"),
                                USN = SafeRead<string>(headers, "USN")
                            };

                            this.notifyMessages.OnNext(response);
                        }
                        else
                        {
                            // TODO: Log
                        }
                    }
                    catch (FormatException ex)
                    {
                        Debug.WriteLine("An error occurred when parsing message from UPnP device \n\nMessage:\n{1}\n\nError:\n{2}:".F(message, ex));
                    }                    
                }
            }
        }

        private static SearchResponseMessage ParseSearchResponseMessage(string message)
        {
            SearchResponseMessage response = null;

            try
            {
                var lines = message.SplitIntoLines();
                if (lines.Any())
                {
                    var statusString = lines[0];
                    if (statusString == "HTTP/1.1 200 OK")
                    {
                        var headers = ParseHeaders(lines.Skip(1));
                        response = new SearchResponseMessage()
                        {
                            BootId = SafeRead<int>(headers, "BOOTID.UPNP.ORG"),
                            ConfigId = SafeRead<int>(headers, "CONFIGID.UPNP.ORG"),
                            Host = SafeRead<string>(headers, "HOST"),
                            Location = SafeRead<string>(headers, "LOCATION"),
                            MaxAge = ParseMaxAge(SafeRead<string>(headers, "CACHE-CONTROL")),
                            SearchTarget = SafeRead<string>(headers, "ST"),
                            SearchPort = SafeRead<int>(headers, "SEARCHPORT.UPNP.ORG"),
                            Server = SafeRead<string>(headers, "SERVER"),
                            USN = SafeRead<string>(headers, "USN")
                        };
                    }
                    else
                    {
                        // TODO: log
                    }
                }
                else
                {
                    // TODO: log
                }
            }
            catch (FormatException ex)
            {
                // TODO: Log an error
                Debug.WriteLine("An error occurred when parsing message from UPnP device \n\nMessage:\n{1}\n\nError:\n{2}:".F(message, ex));
            }

            return response;
        }

        private static TValue SafeRead<TValue>(Dictionary<string, string> headers, string key)
        {
            TValue result;
            string val;

            if (headers.TryGetValue(key.ToUpper(), out val))
            {
                if (typeof (TValue) == typeof (int))
                {
                    result = (TValue) (object) int.Parse(val);
                }
                else if (typeof (TValue) == typeof (NotifyMessageType?))
                {
                    switch (val.ToUpper())
                    {
                        case "SSDP:ALIVE":
                            result = (TValue) (object) NotifyMessageType.Alive;
                            break;
                        case "SSDP:BYEBYE":
                            result = (TValue) (object) NotifyMessageType.ByeBye;
                            break;
                        case "SSDP:UPDATE":
                            result = (TValue) (object) NotifyMessageType.Update;
                            break;
                        default:
                            result = (TValue) (object) null;
                            break;
                    }
                }
                else if (typeof (TValue) == typeof (DateTime))
                {
                    result = (TValue) (object) DateTime.ParseExact(val, RFC1123DateFormat, CultureInfo.InvariantCulture);
                }
                else
                {
                    result = (TValue) (object) val;
                }
            }
            else
            {
                result = default(TValue);
            }

            return result;
        }

        private static int ParseMaxAge(string cacheControlSettings)
        {
            if (string.IsNullOrEmpty(cacheControlSettings) == false)
            {
                var keyValue = cacheControlSettings.Split('=');
                if (keyValue.Length == 2)
                {
                    return Convert.ToInt32(keyValue[1]);
                }
                else
                {
                    throw new FormatException("The cache control settings are in bad format");
                }
            }
            else
            {
                return 0;
            }
        }

        private static Dictionary<string, string> ParseHeaders(IEnumerable<string> headerLines)
        {
            var result = new Dictionary<string, string>();

            foreach (var headerLine in headerLines)
            {
                var keyValue = headerLine.Split(new[] { ":" }, 2, StringSplitOptions.None);

                result[keyValue[0].ToUpper()] = keyValue[1].TrimStart(' ');
            }

            return result;
        }

        #endregion
    }
}
