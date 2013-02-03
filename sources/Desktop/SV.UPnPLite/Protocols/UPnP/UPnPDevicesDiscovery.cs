
namespace SV.UPnPLite.Protocols.UPnP
{
    using SV.UPnPLite.Extensions;
    using SV.UPnPLite.Protocols.SSDP;
    using SV.UPnPLite.Protocols.SSDP.Messages;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Xml.Linq;

    public abstract class UPnPDevicesDiscovery<TDevice> where TDevice : UPnPDevice
    {
        #region Fields

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
            : this(targetDevices, SSDPServer.Instance)
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

        #endregion

        #region Methods

        /// <summary>
        ///     Creates an instance of concrete <see cref="UPnPDevice"/> which manages a device.
        /// </summary>
        /// <param name="udn">
        ///     A universally-unique identifier for the device.
        /// </param>
        /// <param name="services">
        ///     A set of UPnP service found on the device.
        /// </param>
        /// <returns>
        ///     A concrete instance of the <see cref="UPnPDevice"/>.
        /// </returns>
        protected abstract TDevice CreateDeviceInstance(string udn, IEnumerable<UPnPService> services);

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
                    var response = await request.GetResponseAsync();

                    lock (this.availableDevices)
                    {
                        if (availableDevices.ContainsKey(notifyMessage.USN) == false)
                        {
                            var location = new Uri(notifyMessage.Location);
                            var host = "{0}:{1}".F(location.Host, location.Port);

                            var device = CreateDevice(host, response.GetResponseStream());
                            var deviceEx = new DeviceLifetimeControlInfo
                                               {
                                                   Device = device,
                                                   LifeTimeControl = Observable.Timer(TimeSpan.FromSeconds(notifyMessage.MaxAge)).Subscribe(_ => RemoveDevice(notifyMessage.USN))
                                               };

                            Debug.WriteLine("{0} Device '{1}' will live for a '{2}' seconds", DateTime.Now, device.FriendlyName, notifyMessage.MaxAge);
                            availableDevices[notifyMessage.USN] = deviceEx;

                            this.devicesActivity.OnNext(new DeviceActivityEventArgs<TDevice> { Activity = DeviceActivity.Available, Device = device });
                        }
                    }
                }
                catch (WebException ex)
                {
                    // TODO: Log
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
                    Debug.WriteLine("{0} Device '{1}' will live for a '{2}' seconds", DateTime.Now, deviceLifetimeControl.Device.FriendlyName, notifyMessage.MaxAge);

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
                }
            }
        }

        private static string ParseDeviceType(string deviceTypeString)
        {
            string deviceType;

            var lastColonIndex = deviceTypeString.LastIndexOf(':');
            if (lastColonIndex != -1)
            {
                deviceType = deviceTypeString.Substring(0, lastColonIndex);
            }
            else
            {
                deviceType = deviceTypeString;

                // TODO: Log warning
            }

            return deviceType;
        }

        private static UPnPVersion ParseDeviceVersion(string deviceTypeString)
        {
            var version = new UPnPVersion();

            var lastColonIndex = deviceTypeString.LastIndexOf(':');
            if (lastColonIndex != -1)
            {
                var versionString = deviceTypeString.Substring(lastColonIndex + 1, deviceTypeString.Length - lastColonIndex - 1);
                var majorMinorSplit = versionString.Split('.');

                int value;
                if (int.TryParse(majorMinorSplit[0], out value))
                {
                    version.Major = value;
                }
                else
                {
                    version.Major = 1;

                    // TODO: Log warning
                }

                if (majorMinorSplit.Length > 1 && int.TryParse(majorMinorSplit[1], out value))
                {
                    version.Minor = value;
                }
            }
            else
            {
                // TODO: Log warning
            }

            return version;
        }

        private TDevice CreateDevice(string host, Stream deviceDescription)
        {
            var xmlDoc = XDocument.Load(deviceDescription);
            var upnpNamespace = XNamespace.Get("urn:schemas-upnp-org:device-1-0");
            var urlBaseNode = xmlDoc.Element(upnpNamespace + "URLBase");

            Uri baseUri;
            if (urlBaseNode == null || string.IsNullOrWhiteSpace(urlBaseNode.Value))
            {
                baseUri = new Uri("http://{0}".F(host));
            }
            else
            {
                baseUri = new Uri(urlBaseNode.Value);
            }

            var deviceElement = xmlDoc.Descendants(upnpNamespace + "device").First();
            var deviceType = deviceElement.Element(upnpNamespace + "deviceType").Value;
            var deviceUDN = deviceElement.Element(upnpNamespace + "UDN").Value;
            var serciceElements = deviceElement.Descendants(upnpNamespace + "service");

            var services = (from serciceElement in serciceElements
                            let serviceType = serciceElement.Element(upnpNamespace + "serviceType").Value
                            let controlUri = new Uri(baseUri, serciceElement.Element(upnpNamespace + "controlURL").Value)
                            let eventsSunscriptionUri = new Uri(baseUri, serciceElement.Element(upnpNamespace + "eventSubURL").Value)
                            select this.CreateServiceInstance(serviceType, controlUri, eventsSunscriptionUri)).ToList();

            var device = this.CreateDeviceInstance(deviceUDN, services);

            device.FriendlyName = deviceElement.Element(upnpNamespace + "friendlyName").Value;
            device.DeviceVersion = ParseDeviceVersion(deviceType);
            device.Manufacturer = deviceElement.Element(upnpNamespace + "manufacturer").Value;

            device.Icons = (from icon in deviceElement.Descendants(upnpNamespace + "icon")
                            select new DeviceIcon
                                {
                                    Type = icon.Element(upnpNamespace + "mimetype").Value,
                                    Size = new Size
                                        {
                                            Width = int.Parse(icon.Element(upnpNamespace + "width").Value),
                                            Height = int.Parse(icon.Element(upnpNamespace + "height").Value),
                                        },
                                    ColorDepth = icon.Element(upnpNamespace + "depth").Value,
                                    Url = new Uri(baseUri, icon.Element(upnpNamespace + "url").Value)
                                }).ToList();

            return device;
        }

        #endregion

        #region Types

        private class DeviceLifetimeControlInfo
        {
            public TDevice Device { get; set; }

            public IDisposable LifeTimeControl { get; set; }
        }

        #endregion
    }
}
