
namespace SV.UPnPLite.Protocols.SSDP
{
	using System;
	using System.Reactive.Linq;
	using System.Reactive.Subjects;
	using System.Runtime.InteropServices.WindowsRuntime;
	using System.Text;
	using System.Threading.Tasks;
	using SV.UPnPLite.Extensions;
	using SV.UPnPLite.Logging;
	using SV.UPnPLite.Protocols.SSDP.Messages;
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

		private static SSDPServer instance;

		private ILogger logger;

		private readonly Subject<NotifyMessage> notifyMessages = new Subject<NotifyMessage>();

		private DatagramSocket server;

		private HostName multicastHost;

		#endregion

		#region Constructors

		/// <summary>
		///     Initializes a new instance of the <see cref="SSDPServer" /> class.
		/// </summary>
		/// <param name="logManager">
		///     The <see cref="ILogManager"/> to use for logging the debug information.
		/// </param>
		private SSDPServer(ILogManager logManager = null)
		{
			if (logManager != null)
			{
				this.logger = logManager.GetLogger<SSDPServer>();
			}
		}

		#endregion

		#region Properties

		/// <summary>
		///     An observable collection which contains notifications from devices.
		/// </summary>
		public IObservable<NotifyMessage> NotifyMessages { get { return this.notifyMessages; } }

		#endregion

		#region Members

		/// <summary>
		///     Gets a singletone instance of the <see cref="SSDPServer"/>.
		/// </summary>
		/// <param name="logManager">
		///     The <see cref="ILogManager"/> to use for logging the debug information.
		/// </param>
		/// <returns>
		///     A singletone instance of the <see cref="SSDPServer"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="logManager"/> is <c>null</c>.
		/// </exception>
		public static SSDPServer GetInstance(ILogManager logManager = null)
		{
			lock (instanceSyncObject)
			{
				if (instance == null)
				{
					instance = new SSDPServer(logManager);
					instance.StartAync();
				}
			}

			return instance;
		}

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
			return Observable.Create<SearchResponseMessage>(async observer =>
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
					catch (ArgumentException ex)
					{
						logger.Instance().Warning(ex, "The received M-Search response has been ignored.", "Message".As(message));
					}
				};

				await searchSocket.BindServiceNameAsync("0");
				searchSocket.JoinMulticastGroup(this.multicastHost);

				var request = MSearchRequestFormattedString.F(searchTarget, timeForResponse);
				var buffer = Encoding.UTF8.GetBytes(request).AsBuffer();

				// Sending the search request to a multicast group
				var outputStream = await searchSocket.GetOutputStreamAsync(this.multicastHost, MulticastPort.ToString());
				await outputStream.WriteAsync(buffer);
				await outputStream.WriteAsync(buffer);

				// Stop listening for a devices when timeout for responses is expired
				Observable.Timer(TimeSpan.FromSeconds(timeForResponse)).Subscribe(s =>
				{
					observer.OnCompleted();
					searchSocket.Dispose();
				});

				logger.Instance().Debug("M-Search request has been sent. [multicastHost={0}, searchTarget={1}]".F(multicastHost.DisplayName, searchTarget));
				return searchSocket.Dispose;
			});
		}

		private async Task StartAync()
		{
			this.multicastHost = new HostName(MulticastAddress);

			this.server = new DatagramSocket();
			this.server.MessageReceived += this.NotifyMessageReceived;
			await this.server.BindServiceNameAsync(MulticastPort.ToString());
			this.server.JoinMulticastGroup(this.multicastHost);

			this.logger.Instance().Info("Started listening for notification messages", "Port".As(MulticastPort), "MulticastGroup".As(this.multicastHost));
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
			catch (ArgumentException ex)
			{
				logger.Instance().Warning(ex, "The received notification message has been ignored.", "Message".As(message));
			}
		}

		#endregion
	}
}
