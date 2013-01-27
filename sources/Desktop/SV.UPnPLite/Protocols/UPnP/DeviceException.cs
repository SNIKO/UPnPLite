
namespace SV.UPnPLite.Protocols.UPnP
{
    using System;

    public class DeviceException : Exception
    {
        #region Constructors

        public DeviceException(int errorCode, string description)
            : this(errorCode, description, null)
        {
        }

        public DeviceException(int errorCode, string description, Exception innerException)
            : base(description, innerException)
        {
            this.ErrorCode = errorCode;
        }

        #endregion

        #region Properties

        public int ErrorCode { get; private set; }

        #endregion
    }
}
