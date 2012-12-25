
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
                                                     select new DeviceActivityEventArgs<TDevice>
                                                                {
                                                                    Device = this.CreateDevice(activity.Device),
                                                                    Activity = activity.Activity
                                                                };

            var deviceFound = from activity in activityOfDevicesOfTheSpecificType
                              where activity.Activity == DeviceActivity.Available
                              select activity;

            var deviceGone = from activity in activityOfDevicesOfTheSpecificType
                             where activity.Activity == DeviceActivity.Gone
                             select activity;

            deviceFound.Synchronize(this.discoveredDevices).Subscribe(device => this.discoveredDevices.Add(device.Device));
            deviceGone.Synchronize(this.discoveredDevices).Subscribe(device => this.discoveredDevices.Remove(device.Device));

            activityOfDevicesOfTheSpecificType.Subscribe(this.devicesActivity);            
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
