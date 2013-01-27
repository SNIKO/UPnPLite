
namespace SV.UPnPLite.Protocols.DLNA
{
    using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory;
    using SV.UPnPLite.Protocols.UPnP;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    ///     A device which stores a media content.
    /// </summary>
    public class MediaServer : UPnPDevice
    {
        #region Fields

        private readonly IContentDirectoryService contentDirectoryService;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MediaServer" /> class.
        /// </summary>
        /// <param name="udn">
        ///     A universally-unique identifier for the device.
        /// </param>
        /// <param name="contentDirectoryService">
        ///     A <see cref="IContentDirectoryService"/> to use for managing the media content.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="udn"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
        ///     <paramref name="contentDirectoryService"/> is <c>null</c>.
        /// </exception>
        public MediaServer(string udn, IContentDirectoryService contentDirectoryService)
            : base(udn)
        {
            contentDirectoryService.EnsureNotNull("contentDirectoryService");

            this.contentDirectoryService = contentDirectoryService;
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
