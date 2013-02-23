
namespace SV.UPnPLite.Protocols.UPnP
{
    using SV.UPnPLite.Extensions;
    using SV.UPnPLite.Logging;
    using System;
    using System.Collections.Generic;

    /// <summary>    
    ///     Defines members for controlling a UPnP device.
    /// </summary>
    public abstract class UPnPDevice
    {
        #region Fields

        protected readonly ILogger logger;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UPnPDevice"/> class.
        /// </summary>
        /// <param name="udn">
        ///     A universally-unique identifier for the device.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="udn"/> is <c>nukk</c> or <see cref="string.Empty"/>.
        /// </exception>
        protected UPnPDevice(string udn)
        {
            udn.EnsureNotNull("udn");

            this.UDN = udn;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UPnPDevice"/> class.
        /// </summary>
        /// <param name="udn">
        ///     A universally-unique identifier for the device.
        /// </param>
        /// <param name="logManager">
        ///     The <see cref="ILogManager"/> to use for logging the debug information.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="udn"/> is <c>nukk</c> or <see cref="string.Empty"/>.
        /// </exception>
        protected UPnPDevice(string udn, ILogManager logManager)
            : this(udn)
        {
            if (logManager != null)
            {
                this.logger = logManager.GetLogger(this.GetType());
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a type of the device.
        /// </summary>
        public string DeviceType { get; internal set; }

        /// <summary>
        ///     GEts a version of the device.
        /// </summary>
        public UPnPVersion DeviceVersion { get; internal set; }

        /// <summary>
        ///     Gets a name of the device.
        /// </summary>
        public string FriendlyName { get; internal set; }

        /// <summary>
        ///     Gets a manufacturer's name of the device.
        /// </summary>
        public string Manufacturer { get; internal set; }

        /// <summary>
        ///     Gets a universally-unique identifier for the device, whether root or embedded. Must be the same over time for a specific device instance (i.e., must survive reboots).
        /// </summary>
        public string UDN { get; private set; }

        /// <summary>
        ///     Gets a network address of the device.
        /// </summary>
        public string Address { get; internal set; }

        /// <summary>
        ///     Gets the list of icons to depict device in a UI.
        /// </summary>
        public IEnumerable<DeviceIcon> Icons { get; internal set; }

        #endregion
    }
}
