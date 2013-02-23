
namespace SV.UPnPLite.Protocols.UPnP
{
    using SV.UPnPLite.Extensions;
    using System;
    using System.Text;

    /// <summary>
    ///     Defines an error that occurred on a UPnP Device.
    /// </summary>
    /// <typeparam name="TDevice">
    ///     A concrete type of the <see cref="UPnPDevice"/>.
    /// </typeparam>
    public abstract class UPnPDeviceException<TDevice> : Exception where TDevice : UPnPDevice
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UPnPDeviceException{TDevice}"/> class.
        /// </summary>
        /// <param name="device">
        ///     An instance of the device which caused an error.
        /// </param>
        /// <param name="message">
        ///     The message that describes an error.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="device"/> is <c>null</c>.
        /// </exception>
        protected UPnPDeviceException(TDevice device, string message)
            : this(device, message, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UPnPDeviceException{TDevice}"/> class.
        /// </summary>
        /// <param name="device">
        ///     An instance of the device whihc caused an error.
        /// </param>
        /// <param name="message">
        ///     The message that describes an error.
        /// </param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. 
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="device"/> is <c>null</c>.
        /// </exception>
        protected UPnPDeviceException(TDevice device, string message, Exception innerException)
            : base(message, innerException)
        {
            device.EnsureNotNull("device");

            this.Device = device;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a <typeparamref name="TDevice"/> instance which caused an error.
        /// </summary>
        public TDevice Device { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var description = new StringBuilder();
            description.AppendLine("{0}: {1}".F(this.GetType(), this.Message));
            description.AppendLine("Device: address='{0}', name='{1}', udn='{2}'".F(this.Device.Address, this.Device.FriendlyName, this.Device.UDN));

            if (this.InnerException != null)
            {
                description.AppendLine(" ---> {0}".F(this.InnerException));
            }

            if (this.StackTrace != null)
            {
                description.AppendLine();
                description.AppendLine(this.StackTrace);
            }

            return description.ToString();
        }

        #endregion
    }
}
