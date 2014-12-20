
namespace SV.UPnPLite.Protocols.DLNA
{
    using SV.UPnPLite.Extensions;
    using SV.UPnPLite.Logging;
    using SV.UPnPLite.Protocols.DLNA.Extensions;
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

        private readonly List<string> requiredPropertiesFilter = new List<string>
            {
                MediaObject.Properties.Id, 
                MediaObject.Properties.ContainerChildCount
            }; 

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
        ///     Loads root media objects.
        /// </summary>
        /// <returns>
        ///     A list of root media objects.
        /// </returns>
        /// <exception cref="MediaServerException">
        ///     An error occurred when receiving result from media server.
        /// </exception>
        public async Task<IEnumerable<MediaObject>> BrowseAsync()
        {
            try
            {
                var browseResult = await this.contentDirectoryService.BrowseAsync("0", BrowseFlag.BrowseDirectChildren, "*", 0, 0, string.Empty);

                return browseResult.Result;
            }
            catch (FormatException ex)
            {
                throw new MediaServerException(this, MediaServerError.UnexpectedError, "Received result is in a bad format", ex);
            }
            catch (UPnPServiceException ex)
            {
                throw new MediaServerException(this, ex.ErrorCode.ToMediaServerError(), "An error occurred when browsing root objects", ex);
            }
        }

        /// <summary>
        ///     Loads the root media object.
        /// </summary>
        /// <param name="properties">
        ///     The properties of the media objects to load. Use this property to load only needed properties instead of loading all of them. It will reduce the server and network load.
        ///     All properties are listed here: <see cref="MediaObject.Properties"/>.
        /// </param>
        /// <returns>
        ///     A list of root media objects.
        /// </returns>
        /// <exception cref="MediaServerException">
        ///     An error occurred when receiving result from media server.
        /// </exception>
        public async Task<IEnumerable<MediaObject>> BrowseAsync(IEnumerable<string> properties)
        {
            try
            {
                var propertiesFilter = new List<string>();
                propertiesFilter.AddRange(this.requiredPropertiesFilter);

                if (properties != null)
                {
                    propertiesFilter.AddRange(properties);
                }

                var filter = string.Join(",", propertiesFilter);
                var browseResult = await this.contentDirectoryService.BrowseAsync("0", BrowseFlag.BrowseDirectChildren, filter, 0, 0, string.Empty);

                return browseResult.Result;
            }
            catch (FormatException ex)
            {
                throw new MediaServerException(this, MediaServerError.UnexpectedError, "Received result is in a bad format", ex);
            }
            catch (UPnPServiceException ex)
            {
                throw new MediaServerException(this, ex.ErrorCode.ToMediaServerError(), "An error occurred when browsing root folders", ex);
            }
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
        /// <exception cref="MediaServerException">
        ///     An error occurred when receiving result from media server.
        /// </exception>
        public async Task<IEnumerable<MediaObject>> BrowseAsync(MediaContainer container)
        {
            container.EnsureNotNull("container");

            try
            {
                var browseResult = await this.contentDirectoryService.BrowseAsync(container.Id, BrowseFlag.BrowseDirectChildren, "*", 0, 0, string.Empty);
                container.Revision = browseResult.UpdateId;

                return browseResult.Result;
            }
            catch (FormatException ex)
            {
                throw new MediaServerException(this, MediaServerError.UnexpectedError, "Received result is in a bad format", ex);
            }
            catch (UPnPServiceException ex)
            {
                throw new MediaServerException(this, ex.ErrorCode.ToMediaServerError(), "An error occurred when browsing container '{0}'".F(container.Title), ex);
            }
        }

        /// <summary>
        ///     Loads the media objects from <paramref name="container"/>.
        /// </summary>
        /// <param name="container">
        ///     The container from which to load media objects.
        /// </param>
        /// <param name="properties">
        ///     The properties of the media objects to load. Use this property to load only needed properties instead of loading all of them. It will reduce the server and network load.
        ///     All properties are listed here: <see cref="MediaObject.Properties"/>.
        /// </param>
        /// <returns>
        ///     A list of media objects loaded from the <paramref name="container"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="container"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="MediaServerException">
        ///     An error occurred when receiving result from media server.
        /// </exception>
        public async Task<IEnumerable<MediaObject>> BrowseAsync(MediaContainer container, IEnumerable<string> properties)
        {
            container.EnsureNotNull("container");

            try
            {
                var propertiesFilter = new List<string>();
                propertiesFilter.AddRange(this.requiredPropertiesFilter);

                if (properties != null)
                {
                    propertiesFilter.AddRange(properties);
                }

                var filter = string.Join(",", propertiesFilter);
                var browseResult = await this.contentDirectoryService.BrowseAsync(container.Id, BrowseFlag.BrowseDirectChildren, filter, 0, 0, string.Empty);

                return browseResult.Result;
            }
            catch (FormatException ex)
            {
                throw new MediaServerException(this, MediaServerError.UnexpectedError, "Received result is in a bad format", ex);
            }
            catch (UPnPServiceException ex)
            {
                throw new MediaServerException(this, ex.ErrorCode.ToMediaServerError(), "An error occurred when browsing container '{0}'".F(container.Title), ex);
            }
        }

        /// <summary>
        ///     Gets metadata of the container referenced by <paramref name="containerId"/>.
        /// </summary>
        /// <param name="containerId">
        ///     An id of the container on media server.
        /// </param>
        /// <returns>
        ///     A <see cref="MediaContainer"/> instance which defines an information about container.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="containerId"/> is <c>null</c> or <see cref="string.Empty"/>.
        /// </exception>
        /// <exception cref="MediaServerException">
        ///     An error occurred when receiving result from media server.
        /// </exception>
        public async Task<MediaContainer> GetContainerInfoAsync(string containerId)
        {
            containerId.EnsureNotNull("containerId");

            try
            {
                var browseResult = await this.contentDirectoryService.BrowseAsync(containerId, BrowseFlag.BrowseMetadata, "*", 0, 0, string.Empty);
                var newContainer = browseResult.Result.FirstOrDefault() as MediaContainer;
                if (newContainer != null)
                {
                    newContainer.Revision = browseResult.UpdateId;

                    return newContainer;
                }
                else
                {
                    throw new FormatException("Container tag is missing in response");
                }
            }
            catch (FormatException ex)
            {
                throw new MediaServerException(this, MediaServerError.UnexpectedError, "Received result is in a bad format", ex);
            }
            catch (UPnPServiceException ex)
            {
                throw new MediaServerException(this, ex.ErrorCode.ToMediaServerError(), "An error occurred when browsing container '{0}'".F(containerId), ex);
            }
        }

        /// <summary>
        ///     Gets metadata of the item referenced by <paramref name="itemId"/>.
        /// </summary>
        /// <param name="itemId">
        ///     An id of the item on media server.
        /// </param>
        /// <returns>
        ///     A <see cref="MediaContainer"/> instance which defines an information about container.
        /// </returns>
        /// <param name="properties">
        ///     The properties of the media objects to load. Use this property to load only needed properties instead of loading all of them. It will reduce the server and network load.
        ///     All properties are listed here: <see cref="MediaObject.Properties"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="itemId"/> is <c>null</c> or <see cref="string.Empty"/>.
        /// </exception>
        /// <exception cref="MediaServerException">
        ///     An error occurred when receiving result from media server.
        /// </exception>
        public async Task<MediaItem> GetItemInfoAsync(string itemId, params string[] properties)
        {
            itemId.EnsureNotNull("containerId");

            string filter;
            
            if (properties.Any())
            {
                var propertiesFilter = new List<string>();
                propertiesFilter.AddRange(this.requiredPropertiesFilter);
                propertiesFilter.AddRange(properties);

                filter = string.Join(",", propertiesFilter);
            }
            else
            {
                filter = "*";
            }

            try
            {
                var browseResult = await this.contentDirectoryService.BrowseAsync(itemId, BrowseFlag.BrowseMetadata, filter, 0, 0, string.Empty);
                var mediaItem = browseResult.Result.FirstOrDefault() as MediaItem;
                if (mediaItem != null)
                {
                    return mediaItem;
                }
                else
                {
                    throw new FormatException("Container tag is missing in response");
                }
            }
            catch (FormatException ex)
            {
                throw new MediaServerException(this, MediaServerError.UnexpectedError, "Received result is in a bad format", ex);
            }
            catch (UPnPServiceException ex)
            {
                throw new MediaServerException(this, ex.ErrorCode.ToMediaServerError(), "An error occurred when browsing item '{0}'".F(itemId), ex);
            }
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
        /// <exception cref="MediaServerException">
        ///     An error occurred when receiving result from media server.
        /// </exception>
        public async Task<IEnumerable<TMedia>> SearchAsync<TMedia>() where TMedia : MediaItem
        {
            var objectClass = MediaObject.GetClass<TMedia>();

            try
            {
                await this.EnsureSearchCapabilitesReceivedAsync();

                var searchCriteria = "upnp:class derivedfrom \"{0}\"".F(objectClass);
                var searchResult = await this.contentDirectoryService.SearchAsync("0", searchCriteria, "*", 0, 0, string.Empty);

                // TODO: Optimize it
                return searchResult.Result.Select(o => (TMedia)o).GroupBy(m => m.Title).Select(g => g.FirstOrDefault());
            }
            catch (FormatException ex)
            {
                throw new MediaServerException(this, MediaServerError.UnexpectedError, "Received result is in a bad format", ex);
            }
            catch (UPnPServiceException ex)
            {
                throw new MediaServerException(this, ex.ErrorCode.ToMediaServerError(), "An error occurred when searching for items of '{0}' class".F(objectClass), ex);
            }
        }

        private async Task EnsureSearchCapabilitesReceivedAsync()
        {
            if (this.searchCapabilities == null)
            {
                this.searchCapabilities = await this.contentDirectoryService.GetSearchCapabilitiesAsync();
            }
        }

        #endregion
    }
}
