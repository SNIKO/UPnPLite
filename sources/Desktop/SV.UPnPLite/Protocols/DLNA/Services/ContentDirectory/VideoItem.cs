
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
	using SV.UPnPLite.Logging;
	using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory.Extensions;

    /// <summary>
    ///     Represents a piece of content that, when rendered, generates some video. It is atomic in the sense that it does not contain other objects in the ContentDirectory. 
    ///     It typically has at least 1 <see cref="MediaObject.Resources"/> element.
    /// </summary>
    public class VideoItem : MediaItem
    {
        #region Properties

        /// <summary>
        ///     Gets a genre of the media item.
        /// </summary>
        public string Genre { get; internal set; }

        /// <summary>
        ///     Gets a short description of the media item. Description may include but is not limited to: an abstract, 
        ///     a table of contents, a graphical representation, or a free-text account of the resource.
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        ///     Gets a few lines of description of the media item.
        /// </summary>
        public string LongDescription { get; internal set; }

        /// <summary>
        ///     Gets an entity responsible for making the resource available. Examples of a Publisher include a person, an organization, or a service. 
        ///     Typically, the name of a Publisher should be used to indicate the entity.
        /// </summary>
        public string Publisher { get; internal set; }

        /// <summary>
        ///     Gets a name of producer of e.g., a movie or CD.
        /// </summary>
        public string Producer { get; internal set; }

        /// <summary>
        ///     Gets a rating of the object’s resource, for ‘parental control’ filtering purposes, such as “R”, “PG-13”, “X”, etc.,.
        /// </summary>
        public string Rating { get; internal set; }

        /// <summary>
        ///     Gets a name of an actor appearing in a video item.
        /// </summary>
        public string Actor { get; internal set; }

        /// <summary>
        ///     Gets a name of the director of the video content item (e.g., the movie).
        /// </summary>
        public string Director { get; internal set; }

        /// <summary>
        ///     Gets a language of the media item.
        /// </summary>
        public string Language { get; internal set; }

        /// <summary>
        ///     Gets a related resource. Recommended best practice is to identify the related resource by means of a string conforming to a formal identification system.
        /// </summary>
        public string Relation { get; internal set; }

        /// <summary>
        ///     Gets a URL to a album cover.
        /// </summary>
        public string AlbumArtUri { get; internal set; }

        #endregion

		#region Constructors

		/// <summary>
		///		Initializes a new instance of the <see cref="VideoItem"/> class.
		/// </summary>
		/// <param name="logManager">
		///		The log manager to use for logging.
		///	</param>
		public VideoItem(ILogManager logManager = null)
			: base(logManager)
		{
		}

		#endregion

        #region Methods

        /// <summary>
        ///     Sets a value read from an object's metadata XML.
        /// </summary>
        /// <param name="key">
        ///     The key of the property read from XML.
        /// </param>
        /// <param name="value">
        ///     The value of the property read from XML.
        /// </param>
        protected override void SetValue(string key, string value)
        {
            if (key.Is("genre"))
            {
                this.Genre = value;
            }
            else if (key.Is("longDescription"))
            {
                this.LongDescription = value;
            }
            else if (key.Is("producer"))
            {
                this.Producer = value;
            }
            else if (key.Is("rating"))
            {
                this.Rating = value;
            }
            else if (key.Is("actor"))
            {
                this.Actor = value;
            }
            else if (key.Is("director"))
            {
                this.Director = value;
            }
            else if (key.Is("albumArtURI"))
            {
                this.AlbumArtUri = value;
            }
            else if (key.Is("description"))
            {
                this.Description = value;
            }
            else if (key.Is("publisher"))
            {
                this.Publisher = value;
            }
            else if (key.Is("language"))
            {
                this.Language = value;
            }
            else if (key.Is("relation"))
            {
                this.Relation = value;
            }
            else
            {
				base.SetValue(key, value);
            }
        }

        #endregion
    }
}
