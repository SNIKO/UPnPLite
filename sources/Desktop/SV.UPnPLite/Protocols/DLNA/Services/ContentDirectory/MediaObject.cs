
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
    using SV.UPnPLite.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    ///     The base class for all media hosted by Media Server.
    /// </summary>
    public abstract class MediaObject
    {
        #region Fields

        private static Dictionary<string, Type> knownMediaObjectTypes;

        private Dictionary<XName, Action<string>> propertySetters;

        private List<MediaResource> resources;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes the <see cref="MediaObject" /> class.
        /// </summary>
        static MediaObject()
        {
            InitializeMediaObjectTypes();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets an identifier for the object. The value of each object id property must be unique with respect to the Content Directory.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Gets an id property of object’s parent. The parentID of the Content Directory ‘root’ container must be set to the reserved value of  “-1”.  No other 
        ///     parentID attribute of any other Content Directory object may take this value. 
        /// </summary>
        public string ParentId { get; internal set; }

        /// <summary>
        ///     Gets a name of the object.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Gets a primary content creator or owner of the object.
        /// </summary>
        public string Creator { get; internal set; }

        /// <summary>
        ///     Gets a flag indicating whether the object is restricted. When <c>true</c>, ability to modify a given object is confined to the Content Directory Service. 
        ///     Control point metadata write access is disabled.  
        /// </summary>
        public bool Restricted { get; internal set; }

        /// <summary>
        ///     Gets resources associated with media object.
        /// </summary>
        public IEnumerable<MediaResource> Resources { get { return this.resources; } }

        /// <summary>
        ///     Gets a thumbnail of the media item.
        /// </summary>
        public virtual Uri Thumbnail { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Returns a class of the media object. 
        /// </summary>
        /// <typeparam name="TMediaObject">
        ///     The type of the media object for which to get class.
        /// </typeparam>
        /// <returns>
        ///     The class of the media object defined by <typeparamref name="TMediaObject"/>.
        /// </returns>
        public static string GetClass<TMediaObject>() where TMediaObject : MediaObject
        {
            var objectClass = knownMediaObjectTypes.First(pair => pair.Value == typeof (TMediaObject)).Key;

            return objectClass;
        }

        /// <summary>
        ///     Creates a <see cref="MediaObject"/> instance from object defined in DIDL-Lite XML.
        /// </summary>
        /// <param name="didlLiteXml">
        ///     A XML element id DIDL-Lite format which represents a MediaObject.
        /// </param>
        /// <returns>
        ///     A <see cref="MediaObject"/> instance created from the string that contains XML in DIDL-Lite format.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="didlLiteXml"/> is <c>null</c> or <see cref="string.Empty"/>.
        /// </exception>
        public static MediaObject Create(string didlLiteXml)
        {
            didlLiteXml.EnsureNotNull("didlLiteXml");

            var item = XElement.Parse(didlLiteXml);
            var objectClass = item.Element(Namespaces.UPnP + "class").Value;
            var mediaObject = CreateMediaObject(objectClass);
            if (mediaObject != null)
            {
                mediaObject.Deserialize(item.ToString());
            }

            return mediaObject;
        }

        /// <summary>
        ///     Deserilizes the object from a DIDL-Lite XML.
        /// </summary>
        /// <param name="objectXml">
        ///     A source XML created according to DIDL-Lite schema.
        /// </param>
        public void Deserialize(string objectXml)
        {
            this.EnsurePropertySettersInititalized();
            this.resources = new List<MediaResource>();

            var mediaObject = XElement.Parse(objectXml);

            foreach (var attribute in mediaObject.Attributes())
            {
                Action<string> propertySetter;
                if (this.propertySetters.TryGetValue(attribute.Name, out propertySetter))
                {
                    propertySetter(attribute.Value);
                }
            }

            foreach (var element in mediaObject.Elements())
            {
                // TODO: Try to avoid this condition

                if (XNameComparer.OrdinalIgnoreCase.Equals(element.Name, Namespaces.DIDL + "res"))
                {
                    this.resources.Add(new MediaResource().Deserialize(element.ToString()));
                }
                else
                {
                    Action<string> propertySetter;
                    if (this.propertySetters.TryGetValue(element.Name, out propertySetter))
                    {
                        propertySetter(element.Value);
                    }
                }
            }

            if (this.resources.Any() && typeof(ImageItem) == this.GetType())
            {
                this.Thumbnail = new Uri(this.resources[0].Uri);
            }
        }

        /// <summary>
        ///     Initializes delegates which sets the an appropriate properties according to read parameters from XML. 
        /// </summary>
        /// <param name="propertyNameToSetterMap">
        ///     A map between name of the parameter in XML and delegate which sets an appropriate property on object.
        /// </param>
        protected virtual void InitializePropertySetters(Dictionary<XName, Action<string>> propertyNameToSetterMap)
        {
            propertyNameToSetterMap["id"] = value => this.Id = value;
            propertyNameToSetterMap["parentID"] = value => this.ParentId = value;
            propertyNameToSetterMap["restricted"] = value => this.Restricted = value.ToBool();
            propertyNameToSetterMap[Namespaces.DC + "title"] = value => this.Title = value;
            propertyNameToSetterMap[Namespaces.DC + "creator"] = value => this.Creator = value;
        }

        private static void InitializeMediaObjectTypes()
        {
            knownMediaObjectTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            knownMediaObjectTypes["object.item"] = typeof(MediaItem);
            knownMediaObjectTypes["object.item.audioItem"] = typeof(AudioItem);
            knownMediaObjectTypes["object.item.videoItem"] = typeof(VideoItem);
            knownMediaObjectTypes["object.item.imageItem"] = typeof(ImageItem);
            knownMediaObjectTypes["object.container"] = typeof(MediaContainer);
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

        private void EnsurePropertySettersInititalized()
        {
            if (this.propertySetters == null)
            {
                this.propertySetters = new Dictionary<XName, Action<string>>(XNameComparer.OrdinalIgnoreCase);

                this.InitializePropertySetters(this.propertySetters);
            }
        }

        #endregion

        #region Types

        /// <summary>
        ///     Defines some standard XML namespaces.
        /// </summary>
        protected static class Namespaces
        {
            public static XNamespace DIDL = XNamespace.Get("urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");

            public static XNamespace DC = XNamespace.Get("http://purl.org/dc/elements/1.1/");

            public static XNamespace UPnP = XNamespace.Get("urn:schemas-upnp-org:metadata-1-0/upnp/");
        }

        #endregion
    }
}
