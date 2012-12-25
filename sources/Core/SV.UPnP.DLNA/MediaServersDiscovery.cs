
namespace SV.UPnP.DLNA
{
    using System;

    /// <summary>
    ///     Discovers the Media Server devices.
    /// </summary>
    public class MediaServersDiscovery : DLNADevicesDiscovery<MediaServer>, IMediaServersDiscovery
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MediaServersDiscovery" /> class.
        /// </summary>
        /// <param name="upnpDevicesDiscovery">
        ///     The devices discovery service to use for discovering DLNA devices.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="upnpDevicesDiscovery"/> is <c>null</c>.        
        /// </exception>
        public MediaServersDiscovery(IDevicesDiscovery upnpDevicesDiscovery)
            : base(upnpDevicesDiscovery, "urn:schemas-upnp-org:device:MediaServer")
        {
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
        protected override MediaServer CreateDevice(DeviceInfo deviceInfo)
        {
            return new MediaServer(deviceInfo);
        }

        #endregion
    }
}
