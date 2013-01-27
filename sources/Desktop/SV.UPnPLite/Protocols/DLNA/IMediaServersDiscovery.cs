
namespace SV.UPnPLite.Protocols.DLNA
{
    using SV.UPnPLite.Protocols.UPnP;
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Defines methods for discovering Media Server devices.
    /// </summary>
    public interface IMediaServersDiscovery
    {
        /// <summary>
        ///     Gets a list of already discovered devices.
        /// </summary>
        IEnumerable<MediaServer> DiscoveredDevices { get; }

        /// <summary>
        ///     Gets an observable collection which notifies the devices activities.
        /// </summary>
        IObservable<DeviceActivityEventArgs<MediaServer>> DevicesActivity { get; }
    }
}
