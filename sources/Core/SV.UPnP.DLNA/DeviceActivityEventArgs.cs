
namespace SV.UPnP.DLNA
{
    using System;

    /// <summary>
    ///     Defines an information about DLNA device activity.
    /// </summary>
    /// <typeparam name="TDevice">
    ///     The type of the DLNA device.
    /// </typeparam>
    public class DeviceActivityEventArgs<TDevice> : EventArgs where TDevice : DLNADevice
    {
        #region Properties

        /// <summary>
        ///     Gets or sets a device that caused activity.
        /// </summary>
        public TDevice Device { get; set; }

        /// <summary>
        ///     Gets or sets a type of activity of the device.
        /// </summary>
        public DeviceActivity Activity { get; set; }

        #endregion
    }
}
