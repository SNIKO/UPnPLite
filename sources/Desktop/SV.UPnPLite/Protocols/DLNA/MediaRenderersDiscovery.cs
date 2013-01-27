
namespace SV.UPnPLite.Protocols.DLNA
{
    using SV.UPnPLite.Protocols.DLNA.Services.AvTransport;
    using SV.UPnPLite.Protocols.UPnP;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Discovers the Media Renderer devices.
    /// </summary>
    public class MediaRenderersDiscovery : UPnPDevicesDiscovery<MediaRenderer>, IMediaRenderersDiscovery
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MediaRenderersDiscovery" /> class.
        /// </summary>
        public MediaRenderersDiscovery()
            : base("urn:schemas-upnp-org:device:MediaRenderer:1")
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
        protected override MediaRenderer CreateDeviceInstance(string udn, IEnumerable<UPnPService> services)
        {
            var avTransportService = services.FirstOrDefault(s => s is IAvTransportService) as IAvTransportService;

            return new MediaRenderer(udn, avTransportService);
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

            if (serviceType.StartsWith("urn:schemas-upnp-org:service:AVTransport", StringComparison.OrdinalIgnoreCase))
            {
                service = new AvTransportService(serviceType, controlUri, eventsUri);
            }

            return service;
        }

        #endregion
    }
}