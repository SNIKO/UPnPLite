
namespace SV.UPnP.DLNA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using SV.UPnP.DLNA.Services.ContentDirectory;

    /// <summary>
    ///     A device which stores a media content.
    /// </summary>
    public class MediaServer : DLNADevice
    {
        #region Fields

        private readonly ContentDirectoryService contentDirectoryService;

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
        /// <exception cref="ArgumentException">
        ///     One of the following services is required but not exist on device described by <paramref name="deviceInfo"/>:
        ///     <list type="bullet">
        ///         <item>
        ///             ConnectionManager
        ///         </item>
        ///         <item>
        ///             ContentDirectory
        ///         </item>
        ///     </list>
        /// </exception>
        internal MediaServer(DeviceInfo deviceInfo)
            : base(deviceInfo)
        {
            this.contentDirectoryService = new ContentDirectoryService(deviceInfo.Services.FirstOrDefault(s => s.ServiceType.ToUpper().Contains("CONTENTDIRECTORY")));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Loads the root media object.
        /// </summary>
        /// <returns>
        ///     A list of root media objects.
        /// </returns>
        public async Task<IEnumerable<MediaObject>> BrowseAsync()
        {
            var browseResult = await this.contentDirectoryService.BrowseAsync("0", BrowseFlag.BrowseDirectChildren, "*", 0, 0, string.Empty);

            return browseResult.Result;
        }

        /// <summary>
        ///     Loads the media objects from <paramref name="container"/>.
        /// </summary>
        /// <param name="container">
        ///     The container from which to load media objects.        
        /// </param>
        /// <returns>
        ///     A list of media objects loaded from the <paramref name="container"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="container"/> is <c>null</c>.
        /// </exception>
        public async Task<IEnumerable<MediaObject>> BrowseAsync(MediaContainer container)
        {
            container.EnsureNotNull("container");

            var browseResult = await this.contentDirectoryService.BrowseAsync(container.Id, BrowseFlag.BrowseDirectChildren, "*", 0, 0, string.Empty);

            return browseResult.Result;
        }

        #endregion
    }
}
