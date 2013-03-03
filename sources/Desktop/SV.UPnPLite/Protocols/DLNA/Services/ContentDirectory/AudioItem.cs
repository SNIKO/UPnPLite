
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
    using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory.Extensions;

    /// <summary>
    ///     Represents a piece of content that, when rendered, generates some audio2. It is atomic in the sense that it does not contain other objects in the ContentDirectory. 
    ///     It typically has at least 1 <see cref="MediaObject.Resources"/> element.
    /// </summary>
    public class AudioItem : MediaItem
    {
        #region Properties

        /// <summary>
        ///     Gets a genre of the audio item.
        /// </summary>
        public string Genre { get; internal set; }

        /// <summary>
        ///     Gets a short description of the audio item. Description may include but is not limited to: an abstract, 
        ///     a table of contents, a graphical representation, or a free-text account of the resource.
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        ///     Gets a few lines of description of the audio item.
        /// </summary>
        public string LongDescription { get; internal set; }

        /// <summary>
        ///     Gets an entity responsible for making the resource available. Examples of a Publisher include a person, an organization, or a service. 
        ///     Typically, the name of a Publisher should be used to indicate the entity.
        /// </summary>
        public string Publisher { get; internal set; }

        /// <summary>
        ///     Gets a language of the media item.
        /// </summary>
        public string Language { get; internal set; }

        /// <summary>
        ///     Gets a related resource. Recommended best practice is to identify the related resource by means of a string conforming to a formal identification system.
        /// </summary>
        public string Relation { get; internal set; }

        /// <summary>
        ///     Gets an information about rights held in and over the resource.
        /// </summary>
        public string Rights { get; internal set; }

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
        /// <returns>
        ///     <c>true</c>, if the value was set; otherwise, <c>false</c>.
        /// </returns>
        protected override bool TrySetValue(string key, string value)
        {
            if (base.TrySetValue(key, value))
            {
                // The value is set by base object
            }
            else if (key.Is("genre"))
            {
                this.Genre = value;
            }
            else if (key.Is("longDescription"))
            {
                this.LongDescription = value;
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
            else if (key.Is("rights"))
            {
                this.Rights = value;
            }
            else
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
