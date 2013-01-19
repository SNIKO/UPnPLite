
namespace SV.UPnP.DLNA
{
    using SV.UPnP.DLNA.Services.AvTransport;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Discovers the Media Renderer devices.
    /// </summary>
    public class MediaRenderersDiscovery : DevicesDiscovery<MediaRenderer>, IMediaRenderersDiscovery
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
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="udn"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
        ///     <paramref name="services"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     One of the required services is not found in <paramref name="services"/>.
        /// </exception>
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
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="serviceType"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
        ///     <paramref name="controlUri"/> is <c>null</c> -OR-
        ///     <paramref name="eventsUri"/> is <c>null</c>.
        /// </exception>
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