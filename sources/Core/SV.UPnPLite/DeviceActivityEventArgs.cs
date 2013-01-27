
namespace SV.UPnP
{
    using System;

    /// <summary>
    ///     Defines an information about UPnP device activity.
    /// </summary>
    public class DeviceActivityEventArgs<TDevice> : EventArgs where TDevice : UPnPDevice
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
