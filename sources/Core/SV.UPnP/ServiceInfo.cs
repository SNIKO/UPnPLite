
namespace SV.UPnP
{
    using System;

    /// <summary>
    ///     A description for a UPnP service. 
    /// </summary>
    public class ServiceInfo
    {
        #region Properties

        /// <summary>
        ///     Gets a type of the service.
        /// </summary>
        public string ServiceType { get; internal set; }

        /// <summary>
        ///     Gets a URL for service's detailed description.
        /// </summary>
        public string DescriptionURL { get; internal set; }

        /// <summary>
        ///     Gets a URL for controlling the service.
        /// </summary>
        public Uri ControlUri { get; internal set; }

        /// <summary>
        ///     Gets a URL for subscription to service's event.
        /// </summary>
        public Uri EventsSunscriptionUri { get; internal set; }

        #endregion
    }
}
