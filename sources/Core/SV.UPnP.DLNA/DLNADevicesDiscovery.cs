
namespace SV.UPnP.DLNA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    /// <summary>
    ///     The base class for all discoveries of DLNA devices.
    /// </summary>
    /// <typeparam name="TDevice">
    ///     The type of the DLNA device which would be discovered.
    /// </typeparam>
    public abstract class DLNADevicesDiscovery<TDevice> where TDevice : DLNADevice
    {
        #region Fields

        private readonly IDevicesDiscovery upnpDevicesDiscovery;

        private readonly Subject<DeviceActivityEventArgs<TDevice>> devicesActivity;

        private readonly List<TDevice> discoveredDevices;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DLNADevicesDiscovery{TDevice}" /> class.
        /// </summary>
        /// <param name="upnpDevicesDiscovery">
        ///     The devices discovery service to use for discovering DLNA devices.
        /// </param>
        /// <param name="deviceType">
        ///     The type of the DLNA devices to discover.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="upnpDevicesDiscovery"/> is <c>null</c> -OR-
        ///     <paramref name="deviceType"/> is <c>null</c>.
        /// </exception>
        protected DLNADevicesDiscovery(IDevicesDiscovery upnpDevicesDiscovery, string deviceType)
        {
            upnpDevicesDiscovery.EnsureNotNull("upnpDevicesDiscovery");
            deviceType.EnsureNotNull("deviceType");

            this.upnpDevicesDiscovery = upnpDevicesDiscovery;
            this.discoveredDevices = new List<TDevice>(from deviceInfo in this.upnpDevicesDiscovery.DiscoveredDevices select this.CreateDevice(deviceInfo));
            this.devicesActivity = new Subject<DeviceActivityEventArgs<TDevice>>();

            var activityOfDevicesOfTheSpecificType = from activity in this.upnpDevicesDiscovery.DevicesActivity
                                                     where string.Compare(activity.Device.DeviceType, deviceType, StringComparison.OrdinalIgnoreCase) == 0
                                                     select activity;

            var deviceFound = from activity in activityOfDevicesOfTheSpecificType
                              where activity.Activity == DeviceActivity.Available
                              select activity;

            var deviceGone = from activity in activityOfDevicesOfTheSpecificType
                             where activity.Activity == DeviceActivity.Gone
                             select activity;

            deviceFound.Synchronize(this.discoveredDevices).Subscribe(device =>
            {
                var e = new DeviceActivityEventArgs<TDevice>
                {
                    Device = CreateDevice(device.Device),
                    Activity = DeviceActivity.Available
                };
                    
                this.discoveredDevices.Add(e.Device);
                this.devicesActivity.OnNext(e);
            });

            deviceGone.Synchronize(this.discoveredDevices).Subscribe(device =>
            {
                var deviceToRemove = this.discoveredDevices.FirstOrDefault(d => d.UDN == device.Device.UDN);

                if (deviceToRemove != null)
                {
                    var e = new DeviceActivityEventArgs<TDevice>
                    {
                        Device = deviceToRemove,
                        Activity = DeviceActivity.Gone
                    };

                    this.discoveredDevices.Remove(deviceToRemove);
                    this.devicesActivity.OnNext(e);
                }
            });
        }

        #endregion

        #region Properties
        /// <summary>
        ///     Gets the list of already discovered devices.
        /// </summary>
        public IEnumerable<TDevice> DiscoveredDevices
        {
            get
            {
                lock (this.discoveredDevices)
                {
                    return this.discoveredDevices.ToList();
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

        #region Methods
        
        /// <summary>
        ///     Creates a new instance of the class for managing the DLNA device described in <paramref name="deviceInfo"/>.
        /// </summary>
        /// <param name="deviceInfo">
        ///     Defines an information about device.
        /// </param>
        /// <returns>
        ///     An instance of the class which manages device.
        /// </returns>
        protected abstract TDevice CreateDevice(DeviceInfo deviceInfo);

        #endregion
    }
}
