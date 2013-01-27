
namespace SV.UPnPLite.Protocols.SSDP
{
    using SV.UPnPLite.Extensions;
    using SV.UPnPLite.Protocols.SSDP.Messages;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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

        private static readonly object instanceSyncObject = new object();

        private static ISSDPServer instance;

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

        /// <summary>
        ///     Gets a singletone instance of the <see cref="SSDPServer"/>.
        /// </summary>
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

                            try
                            {
                                var response = SearchResponseMessage.Create(message);
                                observer.OnNext(response);
                            }
                            catch (KeyNotFoundException ex)
                            {
                                // TODO: Log
                            }
                            catch (FormatException ex)
                            {
                                Debug.WriteLine("An error occurred when parsing message from UPnP device \n\nMessage:\n{1}\n\nError:\n{2}:".F(message, ex));
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

            try
            {
                var notifyMessage = NotifyMessage.Create(message);

                this.notifyMessages.OnNext(notifyMessage);
            }
            catch (KeyNotFoundException ex)
            {
                // TODO: Log
            }
            catch (FormatException ex)
            {
                Debug.WriteLine("An error occurred when parsing message from UPnP device \n\nMessage:\n{1}\n\nError:\n{2}:".F(message, ex));
            }
        }
        
        #endregion
    }
}
