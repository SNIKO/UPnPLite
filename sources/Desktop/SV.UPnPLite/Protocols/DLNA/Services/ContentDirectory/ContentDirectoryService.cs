
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
    using SV.UPnPLite.Logging;
    using SV.UPnPLite.Protocols.UPnP;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    ///     Enables control of the media on a MediaServer.
    /// </summary>
    public class ContentDirectoryService : UPnPService, IContentDirectoryService
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentDirectoryService" /> class.
        /// </summary>
        /// <param name="serviceType">
        ///     A type of the service.
        /// </param>
        /// <param name="controlUri">
        ///     An URL for sending commands to the service.
        /// </param>
        /// <param name="eventsUri">
        ///     An URL for subscrinbing to service's events.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="serviceType"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
        ///     <paramref name="controlUri"/> is <c>null</c> -OR-
        ///     <paramref name="eventsUri"/> is <c>null</c>.
        /// </exception>
        public ContentDirectoryService(string serviceType, Uri controlUri, Uri eventsUri)
            : base(serviceType, controlUri, eventsUri)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentDirectoryService" /> class.
        /// </summary>
        /// <param name="serviceType">
        ///     A type of the service.
        /// </param>
        /// <param name="controlUri">
        ///     An URL for sending commands to the service.
        /// </param>
        /// <param name="eventsUri">
        ///     An URL for subscrinbing to service's events.
        /// </param>
        /// <param name="logManager">
        ///     The <see cref="ILogManager"/> to use for logging the debug information.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="serviceType"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
        ///     <paramref name="controlUri"/> is <c>null</c> -OR-
        ///     <paramref name="eventsUri"/> is <c>null</c> -OR-
        ///     <paramref name="logManager"/> is <c>null</c>.
        /// </exception>
        public ContentDirectoryService(string serviceType, Uri controlUri, Uri eventsUri, ILogManager logManager)
            : base(serviceType, controlUri, eventsUri, logManager)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Allows the caller to incrementally browse the native hierarchy of the Content Directory objects exposed by the <see cref="ContentDirectoryService"/>, including 
        ///     information listing the classes of objects available in any particular object container. 
        /// </summary>
        /// <param name="objectId">
        ///     Object currently being browsed.  An ObjectID value of zero corresponds to the root object of the Content Directory.
        /// </param>
        /// <param name="browseFlag">
        ///     Specifies a browse option to be used for browsing the Content Directory.
        /// </param>
        /// <param name="filter">
        ///     The comma-separated list of property specifiers (including namespaces) indicates which metadata properties are to be 
        ///     returned in the results from browsing or searching.
        /// </param>
        /// <param name="startingIndex">
        ///     Starting zero based offset to enumerate children under the container specified by <paramref name="objectId"/>. Must be 0 if <paramref name="browseFlag"/> is equal 
        ///     to <see cref="BrowseFlag.BrowseMetadata"/>.
        /// </param>
        /// <param name="requestedCount">
        ///     Requested number of entries under the object specified by <paramref name="objectId"/>. The value '0' indicates request all entries.
        /// </param>
        /// <param name="sortCriteria">
        ///     A CSV list of signed property names, where signed means preceded by ‘+’ or ‘-’ sign.  The ‘+’ and ‘-’Indicate the sort is in ascending or descending order, 
        ///     respectively, with regard to the value of its associated property. Properties appear in the list in order of descending sort priority.
        /// </param>
        /// <exception cref="WebException">
        ///     An error occurred when sending request to service.
        /// </exception>
        /// <exception cref="UPnPServiceException">
        ///     An internal service error occurred when executing request.
        /// </exception>
        public async Task<BrowseResult> BrowseAsync(string objectId, BrowseFlag browseFlag, string filter, int startingIndex, int requestedCount, string sortCriteria)
        {
            var arguments = new Dictionary<string, object>
                                {
                                    {"ObjectID", objectId},
                                    {"BrowseFlag", browseFlag},
                                    {"Filter", filter},
                                    {"StartingIndex", startingIndex},
                                    {"RequestedCount", requestedCount},
                                    {"SortCriteria", sortCriteria},
                                };

            var response = await this.InvokeActionAsync("Browse", arguments);
            var resultXml = response["Result"];
            var mediaObjects = ParseMediaObjects(resultXml);
            var result = new BrowseResult
                             {
                                 Result = mediaObjects,
                                 NumberReturned = Convert.ToInt32(response["NumberReturned"]),
                                 TotalMatches = Convert.ToInt32(response["TotalMatches"]),
                                 UpdateId = Convert.ToInt32(response["UpdateId"])
                             };

            return result;
        }

        /// <summary>
        ///     Searches the content directory for objects that match some search criteria.
        /// </summary>
        /// <param name="containerId">
        ///     The id of the conainer in which to proceed search.
        /// </param>
        /// <param name="searchCriteria">
        ///     One or more search criteria to be used for querying the Content Directory.
        /// </param>
        /// <param name="filter">
        ///     The comma-separated list of property specifiers (including namespaces) indicates which metadata properties are to be 
        ///     returned in the results from browsing or searching.
        /// </param>
        /// <param name="startingIndex">
        ///     Starting zero based offset to enumerate children under the container specified by <paramref name="containerId"/>.
        /// </param>
        /// <param name="requestedCount">
        ///     Requested number of entries under the object specified by <paramref name="containerId"/>. The value '0' indicates request all entries.
        /// </param>
        /// <param name="sortCriteria">
        ///     A CSV list of signed property names, where signed means preceded by ‘+’ or ‘-’ sign.  The ‘+’ and ‘-’Indicate the sort is in ascending or descending order, 
        ///     respectively, with regard to the value of its associated property. Properties appear in the list in order of descending sort priority.
        /// </param>
        /// <returns>
        ///     A <see cref="BrowseResult"/> instance which contains result of the Search operation.
        /// </returns>
        /// <exception cref="WebException">
        ///     An error occurred when sending request to service.
        /// </exception>
        /// <exception cref="UPnPServiceException">
        ///     An internal service error occurred when executing request.
        /// </exception>
        public async Task<BrowseResult> SearchAsync(string containerId, string searchCriteria, string filter, int startingIndex, int requestedCount, string sortCriteria)
        {
            var arguments = new Dictionary<string, object>
                                {
                                    {"ContainerID", containerId},
                                    {"SearchCriteria", searchCriteria},
                                    {"Filter", filter},
                                    {"StartingIndex", startingIndex},
                                    {"RequestedCount", requestedCount},
                                    {"SortCriteria", sortCriteria},
                                };

            var response = await this.InvokeActionAsync("Search", arguments);
            var resultXml = response["Result"];
            var mediaObjects = ParseMediaObjects(resultXml);

            var result = new BrowseResult
            {
                Result = mediaObjects,
                NumberReturned = Convert.ToInt32(response["NumberReturned"]),
                TotalMatches = Convert.ToInt32(response["TotalMatches"]),
                UpdateId = Convert.ToInt32(response["UpdateId"])
            };

            return result;
        }

        /// <summary>
        ///     Returns the searching capabilities that are supported by the device. 
        /// </summary>
        /// <returns>
        ///     The list of property names that can be used in search queries. An empty list indicates that the <see cref="IContentDirectoryService"/> does not support any 
        ///     kind of searching. A wildcard (‘*’) indicates that the device supports search queries using all tags present in the <see cref="IContentDirectoryService"/>.
        /// </returns>
        /// <exception cref="WebException">
        ///     An error occurred when sending request to service.
        /// </exception>
        /// <exception cref="UPnPServiceException">
        ///     An internal service error occurred when executing request.
        /// </exception>
        public async Task<IEnumerable<string>> GetSearchCapabilitiesAsync()
        {
            IEnumerable<string> result;

            var response = await this.InvokeActionAsync("GetSearchCapabilities");
            var propertiesCSV = response["SearchCaps"];

            if (string.IsNullOrWhiteSpace(propertiesCSV) == false)
            {
                result = propertiesCSV.Split(',');
            }
            else
            {
                result = new List<string>();
            }

            return result;
        }

        private static IEnumerable<MediaObject> ParseMediaObjects(string mediaObjectsXml)
        {
            var result = new List<MediaObject>();
            var document = XDocument.Parse(mediaObjectsXml);
            var didl = XNamespace.Get("urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");

            var mediaContainers = document.Descendants(didl + "container").Select(s => s.ToString()).ToList();
            var mediaItems = document.Descendants(didl + "item").Select(s => s.ToString()).ToList();

            var mediaObjects = new List<string>();
            mediaObjects.AddRange(mediaContainers);
            mediaObjects.AddRange(mediaItems);

            foreach (var container in mediaObjects)
            {
                var mediaObject = MediaObject.Create(container);

                if (mediaObject != null)
                {
                    result.Add(mediaObject);
                }
            }

            return result;
        }

        #endregion
    }
}
