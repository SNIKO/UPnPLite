
namespace SV.UPnP
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Xml.Linq;
    using SV.UPnP.Protocols.SSDP;
    using SV.UPnP.Protocols.SSDP.Messages;

    public class DevicesDiscovery : IDevicesDiscovery
    {
        #region Fields

        private readonly ISSDPServer ssdpServer;

        private readonly Subject<DeviceActivityEventArgs> devicesActivity;

        private readonly  Dictionary<string, DeviceLifetimeControlInfo> availableDevices = new Dictionary<string, DeviceLifetimeControlInfo>(); 

        #endregion

        #region Properties
        /// <summary>
        ///     Gets the list of currently discovered devices.
        /// </summary>
        public IEnumerable<DeviceInfo> DiscoveredDevices
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
        public IObservable<DeviceActivityEventArgs> DevicesActivity
        {
            get
            {
                return this.devicesActivity;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DevicesDiscovery" /> class.
        /// </summary>
        public DevicesDiscovery()
            : this("upnp:rootdevice")
        {            
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DevicesDiscovery" /> class.
        /// </summary>
        /// <param name="targetDevices">
        ///     The type of the devices to discover.
        /// </param>
        public DevicesDiscovery(string targetDevices)
            : this(targetDevices, new SSDPServer())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DevicesDiscovery" /> class.
        /// </summary>
        /// <param name="targetDevices">
        ///     The type of the devices to discover.
        /// </param>
        /// <param name="ssdpServer">
        ///     The implementation of the SSDP protocol to use for discovering the UPnP devices.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="ssdpServer"/> is <c>null</c>.
        /// </exception>
        internal DevicesDiscovery(string targetDevices, ISSDPServer ssdpServer)
        {
            ssdpServer.EnsureNotNull();

            this.ssdpServer = ssdpServer;

            this.devicesActivity = new Subject<DeviceActivityEventArgs>();

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

                            this.devicesActivity.OnNext(new DeviceActivityEventArgs { Activity = DeviceActivity.Available, Device = device });
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
                    this.devicesActivity.OnNext(new DeviceActivityEventArgs { Activity = DeviceActivity.Gone, Device = deviceLifetimeControl.Device });
                }
            }
        }

        private static DeviceInfo CreateDevice(string host, Stream deviceDescription)
        {
            var xmlDoc = XDocument.Load(deviceDescription);
            var upnpNamespace = XNamespace.Get("urn:schemas-upnp-org:device-1-0");            
            var urlBaseNode = xmlDoc.Element(upnpNamespace + "URLBase");

            string urlBase;
            if (urlBaseNode == null || string.IsNullOrWhiteSpace(urlBaseNode.Value))
            {
                urlBase = "http://{0}".F(host);
            }
            else
            {
                urlBase = urlBaseNode.Value;
            }


            var deviceInfo = from device in xmlDoc.Descendants(upnpNamespace + "device")
                             select new DeviceInfo()
                                        {
                                            BaseURL = urlBase,
                                            UDN = device.Element(upnpNamespace + "UDN").Value,
                                            DeviceType = device.Element(upnpNamespace + "deviceType").Value,
                                            FriendlyName = device.Element(upnpNamespace + "friendlyName").Value,
                                            Manufacturer = device.Element(upnpNamespace + "manufacturer").Value,
                                            Services = (from service in device.Descendants(upnpNamespace + "service")
                                                       select new ServiceInfo
                                                                  {
                                                                      BaseURL = urlBase,
                                                                      ControlURL = service.Element(upnpNamespace + "controlURL").Value,
                                                                      DescriptionURL = service.Element(upnpNamespace + "SCPDURL").Value,
                                                                      EventsSunscriptionURL = service.Element(upnpNamespace + "eventSubURL").Value,
                                                                      ServiceType = service.Element(upnpNamespace + "serviceType").Value
                                                                  }).ToList()
                                        };

            return deviceInfo.First();
        }

        #endregion

        #region Types

        private class DeviceLifetimeControlInfo
        {
            public DeviceInfo Device { get; set; }

            public IDisposable LifeTimeControl { get; set; }
        }

        #endregion
    }
}
