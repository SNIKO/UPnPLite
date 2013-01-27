
namespace SV.UPnP.DLNA
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Defines methods for discovering Media Renderer devices.
    /// </summary>
    public interface IMediaRenderersDiscovery
    {
        /// <summary>
        ///     Gets a list of already discovered devices.
        /// </summary>
        IEnumerable<MediaRenderer> DiscoveredDevices { get; }

        /// <summary>
        ///     Gets an observable collection which notifies the devices activities.
        /// </summary>
        IObservable<DeviceActivityEventArgs<MediaRenderer>> DevicesActivity { get; }
    }
}
