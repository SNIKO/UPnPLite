
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Xml;
	using SV.UPnPLite.Extensions;
	using SV.UPnPLite.Logging;
	using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory.Extensions;

    /// <summary>
    ///     The base class for all media hosted by Media Server.
    /// </summary>
    public abstract class MediaObject
    {
        #region Fields

        private static Dictionary<string, Type> knownMediaObjectTypes;

        private static readonly XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
        {
            IgnoreComments = true,
            IgnoreWhitespace = true,
            CloseInput = true,
            IgnoreProcessingInstructions = true
        };

        private readonly List<MediaResource> resources = new List<MediaResource>();

		private readonly ILogger logger;

		private readonly ILogManager logManager;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes the <see cref="MediaObject" /> class.
        /// </summary>
		static MediaObject()
        {
            InitializeMediaObjectTypes();
        }

		/// <summary>
		///		Initializes a new instance of the <see cref="MediaObject"/> class.
		/// </summary>
		/// <param name="logManager">
		///		The log manager to use for logging.
		///	</param>
		public MediaObject(ILogManager logManager = null)
		{
			this.logManager = logManager;

			if (logManager != null)
			{
				this.logger = logManager.GetLogger<MediaResource>();
			}
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
        public virtual string ThumbnailUri { get; protected set; }

        /// <summary>
        ///     Gets a UPnP class of the media object.
        /// </summary>
        public string Class
        {
            get { return knownMediaObjectTypes.First(pair => pair.Value == this.GetType()).Key; }
        }

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
            var objectClass = knownMediaObjectTypes.First(pair => pair.Value == typeof(TMediaObject)).Key;

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
        public static MediaObject Create(string didlLiteXml, ILogManager logManager)
        {
            didlLiteXml.EnsureNotNull("didlLiteXml");

            var objectClass = string.Empty;
            using (var reader = new StringReader(didlLiteXml))
            {
                using (var xmlReader = XmlReader.Create(reader, xmlReaderSettings))
                {
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element &&
                            StringComparer.OrdinalIgnoreCase.Compare(xmlReader.LocalName, "class") == 0)
                        {
                            objectClass = xmlReader.ReadElementContentAsString();

                            break;
                        }
                    }
                }
            }

            var mediaObject = CreateMediaObject(objectClass, logManager);
            if (mediaObject != null)
            {
                mediaObject.Deserialize(didlLiteXml);
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
            using (var reader = new StringReader(objectXml))
            {
                using (var xmlReader = XmlReader.Create(reader, xmlReaderSettings))
                {
                    xmlReader.Read();

                    // Reading attribute parameters
                    while (xmlReader.MoveToNextAttribute())
                    {
						try
						{
							this.SetValue(xmlReader.Name, xmlReader.Value);
						}
						catch (FormatException ex)
						{
							this.logger.Instance().Warning(ex, "Unable to parse value '{0}' for key '{1}'.".F(xmlReader.Value, xmlReader.LocalName), "Metadata".As(objectXml));
						}
						catch (OverflowException ex)
						{
							this.logger.Instance().Warning(ex, "Unable to parse value '{0}' for key '{1}'.".F(xmlReader.Value, xmlReader.LocalName), "Metadata".As(objectXml));
						}
                    }

                    xmlReader.MoveToElement();
                    xmlReader.Read();

                    // Reading element parameters
                    while (!xmlReader.EOF)
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            if (xmlReader.LocalName.Is("res"))
                            {
                                this.SetValue("res", xmlReader.ReadOuterXml());
                            }
                            else
                            {
                                this.SetValue(xmlReader.LocalName, xmlReader.ReadElementContentAsString());
                            }
                        }
                        else
                        {
                            xmlReader.Read();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Sets a value read from an object's metadata XML.
        /// </summary>
        /// <param name="key">
        ///     The key of the property read from XML.
        /// </param>
        /// <param name="value">
        ///     The value of the property read from XML.
        /// </param>
        protected virtual void SetValue(string key, string value)
        {
            if (key.Is("id"))
            {
                this.Id = value;
            }
            else if (key.Is("parentID"))
            {
                this.ParentId = value;
            }
            else if (key.Is("restricted"))
            {
                this.Restricted = value.ToBool();
            }
            else if (key.Is("title"))
            {
                this.Title = value;
            }
            else if (key.Is("creator"))
            {
                this.Creator = value;
            }
            else if (key.Is("res"))
            {
                this.resources.Add(new MediaResource(this.logManager).Deserialize(value));
            }
        }

        private static void InitializeMediaObjectTypes()
        {
            knownMediaObjectTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            knownMediaObjectTypes["object.item"] 					  = typeof(MediaItem);
            knownMediaObjectTypes["object.item.audioItem"] 			  = typeof(AudioItem);
            knownMediaObjectTypes["object.item.audioItem.musicTrack"] = typeof(MusicTrack);
            knownMediaObjectTypes["object.item.videoItem"] 			  = typeof(VideoItem);
            knownMediaObjectTypes["object.item.imageItem"] 			  = typeof(ImageItem);
            knownMediaObjectTypes["object.container"] 				  = typeof(MediaContainer);
        }

        private static MediaObject CreateMediaObject(string contentClass, ILogManager logManager)
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
                result = Activator.CreateInstance(mediaObjectType, logManager) as MediaObject;
            }

            return result;
        }

        #endregion

        #region Types

        /// <summary>
        ///     Defines names of the media object in metadata XML.
        /// </summary>
        public static class Properties
        {
            public static string Title = "dc:title";
            public static string Creator = "dc:creator";
            public static string ParentId = "@parentID";
            public static string RefId = "item@refID";
            public static string Restricted = "object@restricted";
            public static string Artist = "upnp:artist";
            public static string Actor = "upnp:actor";
            public static string Producer = "upnp:producer";
            public static string Director = "upnp:director";
            public static string Contributor = "dc:contributor";
            public static string Publisher = "dc:publisher";
            public static string Album = "upnp:album";
            public static string Genre = "upnp:genre";
            public static string AlbumArtUri = "upnp:albumArtURI";
            public static string Relation = "dc:relation";
            public static string StorageMedium = "upnp:storageMedium";
            public static string Description = "dc:description";
            public static string LongDescription = "upnp:longDescription";
            public static string Rating = "upnp:rating";
            public static string Rights = "dc:rights";
            public static string Language = "dc:language";
            public static string Date = "dc:date";

            internal static string Id = "@id";
            internal static string ContainerChildCount = "container@childCount";
            internal static string ContainerSearchable = "container@searchable";
            internal static string Class = "upnp:class";

            public static class Resource
            {
                public static string Uri = "res";
                public static string Size = "res@size";
                public static string Duration = "res@duration";
                public static string Bitrate = "res@bitrate";
                public static string SampleFrequency = "res@sampleFrequency";
                public static string BitsPerSample = "res@bitsPerSample";
                public static string NumberOfAudioChannels = "res@nrAudioChannels";
                public static string Resolution = "res@resolution";
                public static string ColorDepth = "res@colorDepth";
                public static string ProtocolInfo = "res@protocolInfo";
                public static string Protection = "res@protection";
                public static string ImportUri = "res@importUri";
            }
        }

        #endregion
    }
}
