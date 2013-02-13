
namespace SV.UPnPLite.Protocols.DLNA
{
    using SV.UPnPLite.Extensions;
    using SV.UPnPLite.Logging;
    using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory;
    using SV.UPnPLite.Protocols.UPnP;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    ///     A device which stores a media content.
    /// </summary>
    public class MediaServer : UPnPDevice
    {
        #region Fields

        private readonly IContentDirectoryService contentDirectoryService;

        private IEnumerable<string> searchCapabilities;

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

        /// <summary>
        ///     Initializes a new instance of the <see cref="MediaServer" /> class.
        /// </summary>
        /// <param name="udn">
        ///     A universally-unique identifier for the device.
        /// </param>
        /// <param name="contentDirectoryService">
        ///     A <see cref="IContentDirectoryService"/> to use for managing the media content.
        /// </param>
        /// <param name="logManager">
        ///     The <see cref="ILogManager"/> to use for logging the debug information.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="udn"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
        ///     <paramref name="contentDirectoryService"/> is <c>null</c> -OR-
        ///     <paramref name="logManager"/> is <c>null</c>.
        /// </exception>
        public MediaServer(string udn, IContentDirectoryService contentDirectoryService, ILogManager logManager)
            : base(udn, logManager)
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

        /// <summary>
        ///     Searches for a media of type <typeparamref name="TMedia"/>.
        /// </summary>
        /// <typeparam name="TMedia">
        ///     The type of media items to search.
        /// </typeparam>
        /// <returns>
        ///     A list of found media items of type <typeparamref name="TMedia"/>.
        /// </returns>
        public async Task<IEnumerable<TMedia>> SearchAsync<TMedia>() where TMedia : MediaItem
        {
            await this.EnsureSearchCapabilitesReceived();

            var objectClass = MediaObject.GetClass<TMedia>();
            var searchCriteria = "upnp:class derivedfrom \"{0}\"".F(objectClass);
            var searchResult = await this.contentDirectoryService.SearchAsync("0", searchCriteria, "*", 0, 0, string.Empty);

            // TODO: Optimize it
            return searchResult.Result.Select(o => (TMedia) o).GroupBy(m => m.Title).Select(g => g.FirstOrDefault());
        }

        private async Task EnsureSearchCapabilitesReceived()
        {
            if (this.searchCapabilities == null)
            {
                this.searchCapabilities = await this.contentDirectoryService.GetSearchCapabilities();
            }
        }

        #endregion
    }    
}
