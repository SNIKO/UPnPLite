
namespace SV.UPnP
{
    using System;

    public class DeviceActivityEventArgs : EventArgs
    {
        #region Properties

        public DeviceInfo Device { get; set; }

        public DeviceActivity Activity { get; set; }

        #endregion
    }
}
