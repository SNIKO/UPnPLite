
namespace SV.UPnPLite.Protocols.UPnP
{
    using SV.UPnPLite.Extensions;
    using SV.UPnPLite.Logging;
    using SV.UPnPLite.Protocols.SSDP;
    using SV.UPnPLite.Protocols.SSDP.Messages;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Xml.Linq;

    public abstract class UPnPDevicesDiscovery<TDevice> where TDevice : UPnPDevice
    {
        #region Fields

        protected readonly ILogManager logManager;

        protected readonly ILogger logger;

        private readonly ISSDPServer ssdpServer;

        private readonly Subject<DeviceActivityEventArgs<TDevice>> devicesActivity;

        private readonly Dictionary<string, DeviceLifetimeControlInfo> availableDevices = new Dictionary<string, DeviceLifetimeControlInfo>();

        #endregion

        #region Properties
        /// <summary>
        ///     Gets the list of currently discovered devices.
        /// </summary>
        public IEnumerable<TDevice> DiscoveredDevices
        {
            get
            {
                var devices = from keyValuePair in availableDevices
                              select keyValuePair.Value.Device;

                lock (this.availableDevices)
                {
                    return devices.ToList();
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
        /// <param name="targetDevices">
        ///     The type of the devices to discover.
        /// </param>
        /// <param name="ssdpServer">
        ///     The implementation of the SSDP protocol to use for discovering the UPnP devices.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="targetDevices"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
        ///     <paramref name="ssdpServer"/> is <c>null</c>.
        /// </exception>
        internal UPnPDevicesDiscovery(string targetDevices, ISSDPServer ssdpServer)
        {
            targetDevices.EnsureNotNull("targetDevices");
            ssdpServer.EnsureNotNull("ssdpServer");

            this.ssdpServer = ssdpServer;
            this.devicesActivity = new Subject<DeviceActivityEventArgs<TDevice>>();

            var targetDevicesNotifications = from notification in this.ssdpServer.NotifyMessages
                                             where string.Compare(notification.NotificationType, targetDevices, StringComparison.OrdinalIgnoreCase) == 0
                                             select notification;

            targetDevicesNotifications.Where(m => m.NotificationSubtype == NotifyMessageType.Alive).Subscribe(this.AddDevice);
            targetDevicesNotifications.Where(m => m.NotificationSubtype == NotifyMessageType.Update).Subscribe(this.UpdateDevice);
            targetDevicesNotifications.Where(m => m.NotificationSubtype == NotifyMessageType.ByeBye).Subscribe(m => this.RemoveDevice(m.USN));

            this.ssdpServer.Search(targetDevices, 5).Subscribe(this.AddDevice);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UPnPDevicesDiscovery{TDevice}" /> class.
        /// </summary>
        /// <param name="targetDevices">
        ///     The type of the devices to discover.
        /// </param>
        /// <param name="ssdpServer">
        ///     The implementation of the SSDP protocol to use for discovering the UPnP devices.
        /// </param>
        /// <param name="logManager">
        ///     The <see cref="ILogManager"/> to use for logging the debug information.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="targetDevices"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
        ///     <paramref name="ssdpServer"/> is <c>null</c> -OR-
        ///     <paramref name="logManager"/> is <c>null</c>.
        /// </exception>
        internal UPnPDevicesDiscovery(string targetDevices, ISSDPServer ssdpServer, ILogManager logManager)
            : this(targetDevices, ssdpServer)
        {
            this.logManager = logManager;

            if (this.logManager != null)
            {
                this.logger = this.logManager.GetLogger(this.GetType());
                this.logger.Instance().Info("Started listening for a devices' notifications. [targetDevices={0}]", targetDevices);
            }
        }

        #endregion

        #region Methods

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
        ///     A concrete instance of the <see cref="UPnPDevice"/> if all reuqired service available; otherwise, <c>null</c>.
        /// </returns>
        protected abstract TDevice CreateDeviceInstance(string udn, string name, IEnumerable<UPnPService> services);

        /// <summary>
        ///     Creates an instance of concrere <see cref="UPnPService"/> which manages concrete service on a device.
        /// </summary>
        /// <param name="serviceType">
        ///     A type of the service.
        /// </param>
        /// <param name="controlUri">
        ///     An URL for sending commands to the service.
        /// </param>
        /// <param name="eventsUri">
        ///     An URL for subscrinbing to service's events.
        /// </param>
        /// <returns>
        ///     A concrete instance of the <see cref="UPnPService"/>.
        /// </returns>
        protected abstract UPnPService CreateServiceInstance(string serviceType, Uri controlUri, Uri eventsUri);

        private async void AddDevice(SSDPMessage notifyMessage)
        {
            if (availableDevices.ContainsKey(notifyMessage.USN) == false)
            {
                try
                {
                    var request = WebRequest.Create(notifyMessage.Location);
                    using (var response = await request.GetResponseAsync())
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            lock (this.availableDevices)
                            {
                                if (availableDevices.ContainsKey(notifyMessage.USN) == false)
                                {
                                    var location = new Uri(notifyMessage.Location);
                                    var host = "{0}:{1}".F(location.Host, location.Port);

                                    var device = ParseDevice(host, responseStream);
                                    if (device != null)
                                    {
                                        var deviceEx = new DeviceLifetimeControlInfo
                                            {
                                                Device = device,
                                                LifeTimeControl = Observable.Timer(TimeSpan.FromSeconds(notifyMessage.MaxAge)).Subscribe(_ => RemoveDevice(notifyMessage.USN))
                                            };

                                        availableDevices[notifyMessage.USN] = deviceEx;

                                        this.devicesActivity.OnNext(new DeviceActivityEventArgs<TDevice>
                                        {
                                            Activity = DeviceActivity.Available,
                                            Device = device
                                        });

                                        this.logger.Instance().Info(
                                            "Device found. [deviceName={0}, deviceUDN={1}, maxAge={2}, devicesInTotal={3}]",
                                            device.FriendlyName,
                                            device.UDN,
                                            notifyMessage.MaxAge,
                                            availableDevices.Count);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    this.logger.Instance().Warning(ex, "An error occurred when loading description for device '{0}", notifyMessage.USN);
                }
            }
            else
            {
                this.UpdateDevice(notifyMessage);
            }
        }

        private void UpdateDevice(SSDPMessage notifyMessage)
        {
            lock (this.availableDevices)
            {
                DeviceLifetimeControlInfo deviceLifetimeControl;

                if (this.availableDevices.TryGetValue(notifyMessage.USN, out deviceLifetimeControl))
                {
                    this.logger.Instance().Info(
                        "Device just renewed it's lifetime. [deviceName={0}, deviceUDN={1}, maxAge={2}]", 
                        deviceLifetimeControl.Device.FriendlyName,
                        deviceLifetimeControl.Device.UDN,
                        notifyMessage.MaxAge);

                    deviceLifetimeControl.LifeTimeControl.Dispose();
                    deviceLifetimeControl.LifeTimeControl = Observable.Timer(TimeSpan.FromSeconds(notifyMessage.MaxAge)).Subscribe(_ => RemoveDevice(notifyMessage.USN));
                }
            }
        }

        private void RemoveDevice(string deviceUSN)
        {
            lock (this.availableDevices)
            {
                DeviceLifetimeControlInfo deviceLifetimeControl;

                if (this.availableDevices.TryGetValue(deviceUSN, out deviceLifetimeControl))
                {
                    deviceLifetimeControl.LifeTimeControl.Dispose();

                    this.availableDevices.Remove(deviceUSN);
                    this.devicesActivity.OnNext(new DeviceActivityEventArgs<TDevice> { Activity = DeviceActivity.Gone, Device = deviceLifetimeControl.Device });

                    this.logger.Instance().Info(
                        "Device gone. [deviceName={0}, deviceUDN={1}, devicesLeft={2}]", 
                        deviceLifetimeControl.Device.FriendlyName, 
                        deviceLifetimeControl.Device.UDN, 
                        availableDevices.Count);
                }
            }
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

                    this.logger.Instance().Warning("Can't parse major part of device's version. [deviceType={0}]", deviceTypeWithVersion);
                }

                if (majorMinorSplit.Length > 1 && int.TryParse(majorMinorSplit[1], out value))
                {
                    version.Minor = value;
                }
            }
            else
            {
                this.logger.Instance().Warning("The version is missing in device's type. [deviceType={0}]", deviceTypeWithVersion);
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
                    var deviceType          = deviceElement.Element(Namespaces.Device + "deviceType");
                    var deviceUDN           = deviceElement.Element(Namespaces.Device + "UDN");
                    var deviceName          = deviceElement.Element(Namespaces.Device + "friendlyName");
                    var manufactuurer       = deviceElement.Element(Namespaces.Device + "manufacturer");
                    var servicesElement     = deviceElement.Element(Namespaces.Device + "serviceList");
                    var iconsElement        = deviceElement.Element(Namespaces.Device + "iconList");

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
                            "Can't add device as one of required elements is missing. [host={0}, deviceName={1}, deviceType={2}, deviceUDN={3}]",
                            host,
                            deviceName.ValueOrDefault(),
                            deviceType.ValueOrDefault(),
                            deviceUDN.ValueOrDefault());
                    }
                }
                else
                {
                    this.logger.Instance().Warning("Can't add device as it's description is missing. [host={0}]", host);
                }
            }
            else
            {
                this.logger.Instance().Warning("Can't add device as it's description is empty. [host={0}]", host);
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
                    var serviceType             = serviceElement.Element(Namespaces.Device + "serviceType");
                    var controlUri              = serviceElement.Element(Namespaces.Device + "controlURL");
                    var eventsSubscriptionUri   = serviceElement.Element(Namespaces.Device + "eventSubURL");

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
                            "Can't use service as one of the reuqired elemetns is invalid. [serviceType={0}, controlUri={1}, eventsSubscriptionUri={2}, host={3}, deviceName={4}, deviceUDN={5}]",
                            serviceType,
                            controlUri.ValueOrDefault(),
                            eventsSubscriptionUri.ValueOrDefault(),
                            host,
                            deviceName,
                            deviceUDN);
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
                    var width   = iconElement.Element(Namespaces.Device + "width");
                    var height  = iconElement.Element(Namespaces.Device + "height");
                    var uri     = iconElement.Element(Namespaces.Device + "url");
                    var depth   = iconElement.Element(Namespaces.Device + "depth");
                    var type    = iconElement.Element(Namespaces.Device + "mimetype");

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

        private class DeviceLifetimeControlInfo
        {
            public TDevice Device { get; set; }

            public IDisposable LifeTimeControl { get; set; }
        }

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
