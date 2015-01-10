
namespace SV.UPnPLite.Protocols.UPnP
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Reactive.Linq;
	using System.Reactive.Subjects;
	using System.Xml.Linq;
	using SV.UPnPLite.Extensions;
	using SV.UPnPLite.Logging;
	using SV.UPnPLite.Protocols.SSDP;
	using SV.UPnPLite.Protocols.SSDP.Messages;

	public abstract class UPnPDevicesDiscovery<TDevice> : IDisposable where TDevice : UPnPDevice
	{
		#region Constants

		public const int SearchTimeout = 5;

		#endregion

		#region Fields

		protected readonly ILogManager logManager;
		protected readonly ILogger logger;
		private readonly ISSDPServer ssdpServer;
		private readonly string targetDeviceType;
		private readonly Subject<DeviceActivityEventArgs<TDevice>> devicesActivity = new Subject<DeviceActivityEventArgs<TDevice>>();
		private readonly List<TDevice> devices = new List<TDevice>();

		private IDisposable scanProcess;
		private IDisposable expiredDevicesChecker;

		#endregion

		#region Properties
		/// <summary>
		///     Gets the list of currently discovered devices.
		/// </summary>
		public IEnumerable<TDevice> DiscoveredDevices
		{
			get
			{
				lock (this.devices)
				{
					return this.devices.ToList();
				}
			}
		}

		/// <summary>
		///     Gets an observable collection which notifies the devices activities.
		/// </summary>
		public IObservable<DeviceActivityEventArgs<TDevice>> DevicesActivity
		{
			get
			{
				return this.devicesActivity;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		///     Initializes a new instance of the <see cref="UPnPDevicesDiscovery{TDevice}"/> class.
		/// </summary>
		/// <param name="targetDevices">
		///     The type of the devices to discover.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="targetDevices"/> is <c>null</c> or <see cref="string.Empty"/>.
		/// </exception>
		protected UPnPDevicesDiscovery(string targetDevices)
			: this(targetDevices, SSDPServer.GetInstance())
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="UPnPDevicesDiscovery{TDevice}"/> class.
		/// </summary>
		/// <param name="targetDevices">
		///     The type of the devices to discover.
		/// </param>
		/// <param name="logManager">
		///     The <see cref="ILogManager"/> to use for logging the debug information.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="targetDevices"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
		///     <paramref name="logManager"/> is <c>null</c>.
		/// </exception>
		protected UPnPDevicesDiscovery(string targetDevices, ILogManager logManager)
			: this(targetDevices, SSDPServer.GetInstance(logManager), logManager)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="UPnPDevicesDiscovery{TDevice}" /> class.
		/// </summary>
		/// <param name="targetDeviceType">
		///     The type of the devices to discover.
		/// </param>
		/// <param name="ssdpServer">
		///     The implementation of the SSDP protocol to use for discovering the UPnP devices.
		/// </param>
		/// <param name="logManager">
		///     The <see cref="ILogManager"/> to use for logging the debug information.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="targetDeviceType"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
		///     <paramref name="ssdpServer"/> is <c>null</c> -OR-
		///     <paramref name="logManager"/> is <c>null</c>.
		/// </exception>
		internal UPnPDevicesDiscovery(string targetDeviceType, ISSDPServer ssdpServer, ILogManager logManager = null)
		{
			targetDeviceType.EnsureNotNull("targetDevices");
			ssdpServer.EnsureNotNull("ssdpServer");

			this.targetDeviceType = targetDeviceType;
			this.ssdpServer = ssdpServer;
			this.logManager = logManager;

			if (this.logManager != null)
			{
				this.logger = this.logManager.GetLogger(this.GetType());
				this.logger.Instance().Info("Started listening for upnp devices", "TargetDevices".As(targetDeviceType));
			}

			var targetDevicesNotifications = from notification in this.ssdpServer.NotifyMessages
											 where string.Compare(notification.NotificationType, targetDeviceType, StringComparison.OrdinalIgnoreCase) == 0
											 select notification;

			targetDevicesNotifications.Where(m => m.NotificationSubtype == NotifyMessageType.Alive).Subscribe(this.TryAddDevice);
			targetDevicesNotifications.Where(m => m.NotificationSubtype == NotifyMessageType.Update).Subscribe(this.TryUpdateDevice);
			targetDevicesNotifications.Where(m => m.NotificationSubtype == NotifyMessageType.ByeBye).Subscribe(m => this.TryRemoveDevice(m.UDN));

			this.ScanAsync();
		}

		#endregion

		#region Methods

		#region IDisposable

		/// <summary>
		///		Releases unmanaged and - optionally - managed resources.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
		}

		#endregion

		/// <summary>
		///     Creates an instance of concrete <see cref="UPnPDevice"/> which manages a device.
		/// </summary>
		/// <param name="udn">
		///     A universally-unique identifier for the device.
		/// </param>
		/// <param name="name">
		///     A friendly name of the device.
		/// </param>
		/// <param name="services">
		///     A set of UPnP service found on the device.
		/// </param>
		/// <returns>
		///     A concrete instance of the <see cref="UPnPDevice"/> if all required service available; otherwise, <c>null</c>.
		/// </returns>
		protected abstract TDevice CreateDeviceInstance(string udn, string name, IEnumerable<UPnPService> services);

		/// <summary>
		///     Creates an instance of concrete <see cref="UPnPService"/> which manages concrete service on a device.
		/// </summary>
		/// <param name="serviceType">
		///     A type of the service.
		/// </param>
		/// <param name="controlUri">
		///     An URL for sending commands to the service.
		/// </param>
		/// <param name="eventsUri">
		///     An URL for subscribing to service's events.
		/// </param>
		/// <returns>
		///     A concrete instance of the <see cref="UPnPService"/>.
		/// </returns>
		protected abstract UPnPService CreateServiceInstance(string serviceType, Uri controlUri, Uri eventsUri);

		/// <summary>
		///		Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="isDisposing">
		///		<c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
		///	</param>
		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				var scan = this.scanProcess;
				if (scan != null)
				{
					scan.Dispose();
				}

				var checker = this.expiredDevicesChecker;
				if (checker != null)
				{
					checker.Dispose();
				}
			}
		}

		private async void TryAddDevice(SSDPMessage notifyMessage)
		{
			if (this.IsDeviceAdded(notifyMessage.UDN))
			{
				this.TryUpdateDevice(notifyMessage);
			}
			else
			{
				try
				{
					var request = WebRequest.Create(notifyMessage.Location);

					using (var response = await request.GetResponseAsync())
					{
						using (var responseStream = response.GetResponseStream())
						{
							lock (this.devices)
							{
								if (this.IsDeviceAdded(notifyMessage.UDN) == false)
								{
									var location = new Uri(notifyMessage.Location);
									var host = "{0}:{1}".F(location.Host, location.Port);

									var device = ParseDevice(host, responseStream);
									if (device != null)
									{
										device.MaxAge = TimeSpan.FromSeconds(notifyMessage.MaxAge);
										device.LastCheckTime = DateTime.UtcNow;

										this.devices.Add(device);
										this.logger.Instance().Info("The device has been added", "DeviceName".As(device.FriendlyName), "DeviceUDN".As(device.UDN));
										this.devicesActivity.OnNext(new DeviceActivityEventArgs<TDevice> { Activity = DeviceActivity.Available, Device = device });

										this.ScheduleNextCheckForExpiredDevices();
									}
								}
							}
						}
					}
				}
				catch (WebException ex)
				{
					this.logger.Instance().Warning(ex, "Failed to load description for device with UDN '{0}'".F(notifyMessage.UDN));
				}
			}
		}

		private void TryUpdateDevice(SSDPMessage notifyMessage)
		{
			lock (this.devices)
			{
				var device = GetDevice(notifyMessage.UDN);

				if (device != null)
				{
					device.MaxAge = TimeSpan.FromSeconds(notifyMessage.MaxAge);
					device.LastCheckTime = DateTime.UtcNow;

					this.ScheduleNextCheckForExpiredDevices();

					this.logger.Instance().Info("The device lifetime has been updated", "DeviceName".As(device.FriendlyName), "DeviceUDN".As(device.UDN), "MaxAge".As(notifyMessage.MaxAge));
				}
			}
		}

		private void TryRemoveDevice(string deviceUDN)
		{
			lock (this.devices)
			{
				var device = GetDevice(deviceUDN);

				if (device != null)
				{
					this.devices.Remove(device);
					this.logger.Instance().Info("The device has been removed", "DeviceName".As(device.FriendlyName), "DeviceUDN".As(device.UDN));

					this.devicesActivity.OnNext(new DeviceActivityEventArgs<TDevice> { Activity = DeviceActivity.Gone, Device = device });
					this.ScheduleNextCheckForExpiredDevices();
				}
			}
		}

		private void ScanAsync()
		{
			lock (this.ssdpServer)
			{
				if (scanProcess == null)
				{
					this.logger.Instance().Info("Searching for new devices...");

					scanProcess = this.ssdpServer.Search(this.targetDeviceType, SearchTimeout).Subscribe(
						response =>
						{
							this.TryAddDevice(response);
						},
						() =>
						{
							scanProcess = null;
							RemoveExpiredDevices();
						});
				}
			}
		}

		private void RemoveExpiredDevices()
		{
			lock (this.devices)
			{
				var expiredDevices = this.devices.Where(d => DateTime.UtcNow - d.LastCheckTime > d.MaxAge);

				foreach (var expiredDevice in expiredDevices)
				{
					TryRemoveDevice(expiredDevice.UDN);
				}
			}
		}

		private void ScheduleNextCheckForExpiredDevices()
		{
			lock (this.devices)
			{
				if (this.devices.Any())
				{
					var nextCheckTime = this.devices.Select(d => d.LastCheckTime.Add(d.MaxAge)).Min();

					if (expiredDevicesChecker != null)
					{
						expiredDevicesChecker.Dispose();
					}

					expiredDevicesChecker = Observable.Timer(nextCheckTime - DateTime.UtcNow).Subscribe(_ =>
					{
						this.logger.Instance().Debug("The lifetime has been expired for one of the devices");
						this.ScanAsync();
					});
				}
				else
				{
					if (expiredDevicesChecker != null)
					{
						expiredDevicesChecker.Dispose();
					}
				}
			}
		}

		private bool IsDeviceAdded(string deviceUDN)
		{
			return this.devices.Any(d => StringComparer.OrdinalIgnoreCase.Compare(d.UDN, deviceUDN) == 0);
		}

		private TDevice GetDevice(string deviceUDN)
		{
			return this.devices.FirstOrDefault(d => StringComparer.OrdinalIgnoreCase.Compare(d.UDN, deviceUDN) == 0);
		}

		private UPnPVersion ParseDeviceVersion(string deviceTypeWithVersion)
		{
			var version = new UPnPVersion();

			var lastColonIndex = deviceTypeWithVersion.LastIndexOf(':');
			if (lastColonIndex != -1)
			{
				var versionString = deviceTypeWithVersion.Substring(lastColonIndex + 1, deviceTypeWithVersion.Length - lastColonIndex - 1);
				var majorMinorSplit = versionString.Split('.');

				int value;
				if (int.TryParse(majorMinorSplit[0], out value))
				{
					version.Major = value;
				}
				else
				{
					version.Major = 1;

					this.logger.Instance().Warning("Can't parse major part of the device's version", "DeviceType".As(deviceTypeWithVersion));
				}

				if (majorMinorSplit.Length > 1 && int.TryParse(majorMinorSplit[1], out value))
				{
					version.Minor = value;
				}
			}
			else
			{
				this.logger.Instance().Warning("The version is missing in device's type", "DeviceType".As(deviceTypeWithVersion));
			}

			return version;
		}

		private string ParseDeviceType(string deviceTypeWithVersion)
		{
			var type = deviceTypeWithVersion.Substring(0, deviceTypeWithVersion.LastIndexOf(':'));

			return type;
		}

		private TDevice ParseDevice(string host, Stream deviceDescription)
		{
			TDevice device = null;
			var xmlDoc = XDocument.Load(deviceDescription);

			if (xmlDoc.Root != null)
			{
				Uri baseUri;
				var urlBaseNode = xmlDoc.Root.Element(Namespaces.Device + "URLBase");
				if (urlBaseNode == null || string.IsNullOrWhiteSpace(urlBaseNode.Value))
				{
					baseUri = new Uri("http://{0}".F(host));
				}
				else
				{
					baseUri = new Uri(urlBaseNode.Value);
				}

				var deviceElement = xmlDoc.Root.Element(Namespaces.Device + "device");
				if (deviceElement != null)
				{
					var deviceType = deviceElement.Element(Namespaces.Device + "deviceType");
					var deviceUDN = deviceElement.Element(Namespaces.Device + "UDN");
					var deviceName = deviceElement.Element(Namespaces.Device + "friendlyName");
					var manufactuurer = deviceElement.Element(Namespaces.Device + "manufacturer");
					var servicesElement = deviceElement.Element(Namespaces.Device + "serviceList");
					var iconsElement = deviceElement.Element(Namespaces.Device + "iconList");

					if (deviceType != null && deviceUDN != null && deviceName != null)
					{
						var services = this.ParseServices(host, deviceName.Value, deviceUDN.Value, baseUri, servicesElement);

						device = this.CreateDeviceInstance(deviceUDN.Value, deviceName.Value, services);
						if (device != null)
						{
							device.Address = host;
							device.DeviceType = ParseDeviceType(deviceType.Value);
							device.DeviceVersion = ParseDeviceVersion(deviceType.Value);
							device.FriendlyName = deviceName.Value;
							device.Manufacturer = manufactuurer.ValueOrDefault();
							device.Icons = this.ParseIcons(baseUri, iconsElement);
						}
					}
					else
					{
						this.logger.Instance().Warning(
							"The device has been ignored as some mandatory fields in its description are missing",
							"DeviceHost".As(host),
							"DeviceName".As(deviceName.ValueOrDefault()),
							"DeviceType".As(deviceType.ValueOrDefault()),
							"DeviceUDN".As(deviceUDN.ValueOrDefault()));
					}
				}
				else
				{
					this.logger.Instance().Warning("The device has been ignored as its description is missing", "DeviceHost".As(host));
				}
			}
			else
			{
				this.logger.Instance().Warning("The device has been ignored as its description is missing", "DeviceHost".As(host));
			}

			return device;
		}

		private IEnumerable<UPnPService> ParseServices(string host, string deviceName, string deviceUDN, Uri baseUri, XElement servicesListElement)
		{
			var services = new List<UPnPService>();

			if (servicesListElement != null)
			{
				var serviceElements = servicesListElement.Descendants(Namespaces.Device + "service");

				foreach (var serviceElement in serviceElements)
				{
					var serviceType = serviceElement.Element(Namespaces.Device + "serviceType");
					var controlUri = serviceElement.Element(Namespaces.Device + "controlURL");
					var eventsSubscriptionUri = serviceElement.Element(Namespaces.Device + "eventSubURL");

					Uri completeControlUri;
					Uri completeSubscriptionUri;

					Uri.TryCreate(baseUri, controlUri.ValueOrDefault(), out completeControlUri);
					Uri.TryCreate(baseUri, eventsSubscriptionUri.ValueOrDefault(), out completeSubscriptionUri);

					if (serviceType != null && completeControlUri != null && completeSubscriptionUri != null)
					{
						var service = this.CreateServiceInstance(serviceType.Value, completeControlUri, completeSubscriptionUri);

						services.Add(service);
					}
					else
					{
						this.logger.Instance().Warning(
							"The device service has been ignored as some mandatory fields are missing in its description",
							"DeviceHost".As(host),
							"DeviceName".As(deviceName),
							"DeviceUDN".As(deviceUDN),
							"ServiceType".As(serviceType),
							"ControlUri".As(controlUri.ValueOrDefault()),
							"EventsSubscriptionUri".As(eventsSubscriptionUri.ValueOrDefault()));
					}
				}
			}

			return services;
		}

		private IEnumerable<DeviceIcon> ParseIcons(Uri baseUri, XElement iconsListElement)
		{
			var icons = new List<DeviceIcon>();

			if (iconsListElement != null)
			{
				var iconElements = iconsListElement.Descendants(Namespaces.Device + "icon");

				foreach (var iconElement in iconElements)
				{
					var width = iconElement.Element(Namespaces.Device + "width");
					var height = iconElement.Element(Namespaces.Device + "height");
					var uri = iconElement.Element(Namespaces.Device + "url");
					var depth = iconElement.Element(Namespaces.Device + "depth");
					var type = iconElement.Element(Namespaces.Device + "mimetype");

					Uri completeUri;
					if (uri != null && Uri.TryCreate(baseUri, uri.Value, out completeUri))
					{
						var icon = new DeviceIcon();
						icon.Url = completeUri;
						icon.Type = type.ValueOrDefault();
						icon.ColorDepth = depth.ValueOrDefault();

						int parsedWidth;
						int parsedHeight;
						if (width != null && height != null && int.TryParse(width.Value, out parsedWidth) && int.TryParse(height.Value, out parsedHeight))
						{
							icon.Size = new Size(parsedWidth, parsedHeight);
						}

						icons.Add(icon);
					}
				}
			}

			return icons;
		}

		#endregion

		#region Types

		/// <summary>
		///     Defines some standard XML namespaces.
		/// </summary>
		protected static class Namespaces
		{
			public static XNamespace Device = XNamespace.Get("urn:schemas-upnp-org:device-1-0");
		}

		#endregion
	}
}
