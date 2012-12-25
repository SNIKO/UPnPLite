
namespace SV.UPnP.DLNA
{
    using System;

    /// <summary>
    ///     Discovers the Media Renderer devices.
    /// </summary>
    public class MediaRenderersDiscovery : DLNADevicesDiscovery<MediaRenderer>, IMediaRenderersDiscovery
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MediaRenderersDiscovery" /> class.
        /// </summary>
        /// <param name="upnpDevicesDiscovery">
        ///     The devices discovery service to use for discovering DLNA devices.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="upnpDevicesDiscovery"/> is <c>null</c>.        
        /// </exception>
        public MediaRenderersDiscovery(IDevicesDiscovery upnpDevicesDiscovery)
            : base(upnpDevicesDiscovery, "urn:schemas-upnp-org:device:MediaRenderer")
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
        protected override MediaRenderer CreateDevice(DeviceInfo deviceInfo)
        {
            return new MediaRenderer(deviceInfo);
        }

        #endregion
    }
}