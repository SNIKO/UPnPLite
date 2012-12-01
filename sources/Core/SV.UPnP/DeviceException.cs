
namespace SV.UPnP
{
    using System;

    public class DeviceException : Exception
    {
        #region Constructors

        public DeviceException(DeviceError error, string description)
            : this(error, description, null)
        {
        }

        public DeviceException(DeviceError error, string description, Exception innerException)
            : base(description, innerException)
        {
            this.Error = error;
        }

        #endregion

        #region Properties

        public DeviceError Error { get; private set; }

        #endregion
    }
}
