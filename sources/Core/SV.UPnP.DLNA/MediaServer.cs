
namespace SV.UPnP.DLNA
{
    using System;
    using SV.UPnP.DLNA.Services.ContentDirectory;

    /// <summary>
    ///     A device which stores a media content.
    /// </summary>
    public class MediaServer : DLNADevice
    {        
        #region Fields

        private ContentDirectoryService contentDirectoryService;

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
        internal MediaServer(DeviceInfo deviceInfo)
            : base(deviceInfo)
        {
        }

        #endregion

        #region Properties

        #endregion
    }
}
