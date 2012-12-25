
namespace SV.UPnP.DLNA
{
    using System;

    /// <summary>
    ///     A device which renders content from Media Server.
    /// </summary>
    public class MediaRenderer : DLNADevice
    {
        #region Fields
        
        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DLNADevice" /> class.
        /// </summary>
        /// <param name="deviceInfo">
        ///     Defines parameters of the device.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="deviceInfo"/> is <c>null</c>.
        /// </exception>
        internal MediaRenderer(DeviceInfo deviceInfo)
            : base(deviceInfo)
        {
        }

        #endregion

        #region Properties

        #endregion
    }
}
