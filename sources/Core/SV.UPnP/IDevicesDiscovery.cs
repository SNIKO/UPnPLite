
namespace SV.UPnP
{
    using System;
    using System.Collections.Generic;

    public interface IDevicesDiscovery
    {
        /// <summary>
        ///     Gets the list of currently discovered devices.
        /// </summary>
        IEnumerable<DeviceInfo> DiscoveredDevices { get; }

        /// <summary>
        ///     Gets an observable collection which notifies the devices activities.
        /// </summary>
        IObservable<DeviceActivityEventArgs> DevicesActivity { get; }
    }
}