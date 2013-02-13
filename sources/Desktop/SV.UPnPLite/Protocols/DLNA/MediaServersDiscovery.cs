
namespace SV.UPnPLite.Protocols.DLNA
{
    using SV.UPnPLite.Logging;
    using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory;
    using SV.UPnPLite.Protocols.UPnP;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Discovers the Media Server devices.
    /// </summary>
    public class MediaServersDiscovery : UPnPDevicesDiscovery<MediaServer>, IMediaServersDiscovery
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MediaServersDiscovery" /> class.
        /// </summary>
        public MediaServersDiscovery()
            : base("urn:schemas-upnp-org:device:MediaServer:1")
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MediaServersDiscovery" /> class.
        /// </summary>
        /// <param name="logManager">
        ///     The <see cref="ILogManager"/> to use for logging the debug information.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="logManager"/> is <c>null</c>.
        /// </exception>
        public MediaServersDiscovery(ILogManager logManager)
            : base("urn:schemas-upnp-org:device:MediaServer:1", logManager)
        {
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
        protected override MediaServer CreateDeviceInstance(string udn, IEnumerable<UPnPService> services)
        {
            var avTransportService = services.FirstOrDefault(s => s is IContentDirectoryService) as IContentDirectoryService;

            return new MediaServer(udn, avTransportService, this.logManager);
        }

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
        protected override UPnPService CreateServiceInstance(string serviceType, Uri controlUri, Uri eventsUri)
        {
            UPnPService service = null;

            if (serviceType.StartsWith("urn:schemas-upnp-org:service:ContentDirectory", StringComparison.OrdinalIgnoreCase))
            {
                service = new ContentDirectoryService(serviceType, controlUri, eventsUri, this.logManager);
            }

            return service;
        }

        #endregion
    }
}
