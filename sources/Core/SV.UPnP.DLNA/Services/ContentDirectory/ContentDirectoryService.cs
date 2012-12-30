﻿
namespace SV.UPnP.DLNA.Services.ContentDirectory
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class ContentDirectoryService : ServiceBase
    {
        #region Fields

        private static Dictionary<string, Type> knownMediaObjectTypes;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentDirectoryService" /> class.
        /// </summary>
        /// <param name="serviceInfo">
        ///     Defines parameters of the service.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="serviceInfo"/> is <c>null</c>.
        /// </exception>
        public ContentDirectoryService(ServiceInfo serviceInfo)
            : base(serviceInfo)
        {
            InitializeMediaObjectTypes();
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
        /// <returns>
        ///     A <see cref="BrowseResult"/> instance which contains result of the Browse operation.
        /// </returns>
        /// <exception cref="DeviceException">
        ///     An error occurred when sending request to device -OR-
        ///     An error occurred when executing request on device -OR-
        ///     The specified <paramref name="objectId"/> is invalid -OR-
        ///     The sort criteria specified is not supported or is invalid.
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

        private static IEnumerable<MediaObject> ParseMediaObjects(string mediaObjectsXml)
        {
            var result = new List<MediaObject>();
            var document = XDocument.Parse(mediaObjectsXml);
            var didl = XNamespace.Get("urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");
            var upnp = XNamespace.Get("urn:schemas-upnp-org:metadata-1-0/upnp/");

            var containers = document.Descendants(didl + "container");
            foreach (var container in containers)
            {
                var objectClass = container.Element(upnp + "class").Value;
                var mediaObject = CreateMediaObject(objectClass);
                if (mediaObject != null)
                {
                    mediaObject.Deserialize(container.ToString());
                }

                result.Add(mediaObject);
            }

            var items = document.Descendants(didl + "item");
            foreach (var item in items)
            {
                var objectClass = item.Element(upnp + "class").Value;
                var mediaObject = CreateMediaObject(objectClass);
                if (mediaObject != null)
                {
                    mediaObject.Deserialize(item.ToString());
                }

                result.Add(mediaObject);
            }

            return result;
        }

        private static void InitializeMediaObjectTypes()
        {
            if (knownMediaObjectTypes == null)
            {
                knownMediaObjectTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

                knownMediaObjectTypes["object.item"] = typeof(MediaItem);
                knownMediaObjectTypes["object.item.audioItem"] = typeof(AudioItem);
                knownMediaObjectTypes["object.item.videoItem"] = typeof(VideoItem);
                knownMediaObjectTypes["object.item.imageItem"] = typeof(ImageItem);
                knownMediaObjectTypes["object.container"] = typeof(MediaContainer);
            }
        }

        private static MediaObject CreateMediaObject(string contentClass)
        {
            MediaObject result = null;
            Type mediaObjectType;

            if (knownMediaObjectTypes.TryGetValue(contentClass, out mediaObjectType) == false)
            {
                // Type for object with such class not found. Lets fine the closest type. 
                var maxCoincidence = 0;
                foreach (var knownMediaObjectType in knownMediaObjectTypes)
                {
                    if (contentClass.StartsWith(knownMediaObjectType.Key, StringComparison.OrdinalIgnoreCase) && knownMediaObjectType.Key.Length > maxCoincidence)
                    {
                        maxCoincidence = knownMediaObjectType.Key.Length;
                        mediaObjectType = knownMediaObjectType.Value;
                    }
                }
            }

            if (mediaObjectType != null)
            {
                result = Activator.CreateInstance(mediaObjectType) as MediaObject;
            }

            return result;
        }

        #endregion
    }
}
