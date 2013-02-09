
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Represents a piece of content that, when rendered, generates some video. It is atomic in the sense that it does not contain other objects in the ContentDirectory. 
    ///     It typically has at least 1 <res> element.
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
        ///     Gets a name of an actor appearing in a video item
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
        

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes delegates which sets the an appropriate properties according to read parameters from XML. 
        /// </summary>
        /// <param name="propertyNameToSetterMap">
        ///     A map between name of the parameter in XML and delegate which sets an appropriate property on object.
        /// </param>
        protected override void InitializePropertySetters(Dictionary<System.Xml.Linq.XName, Action<string>> propertyNameToSetterMap)
        {
            base.InitializePropertySetters(propertyNameToSetterMap);

            propertyNameToSetterMap[Namespaces.UPnP + "genre"]              = value => this.Genre = value;
            propertyNameToSetterMap[Namespaces.UPnP + "longDescription"]    = value => this.LongDescription = value;
            propertyNameToSetterMap[Namespaces.UPnP + "producer"]           = value => this.Producer = value;
            propertyNameToSetterMap[Namespaces.UPnP + "rating"]             = value => this.Rating = value;
            propertyNameToSetterMap[Namespaces.UPnP + "actor"]              = value => this.Actor = value;
            propertyNameToSetterMap[Namespaces.UPnP + "director"]           = value => this.Director = value;
            propertyNameToSetterMap[Namespaces.DC   + "description"]        = value => this.Description = value;            
            propertyNameToSetterMap[Namespaces.DC   + "publisher"]          = value => this.Publisher = value;
            propertyNameToSetterMap[Namespaces.DC   + "language"]           = value => this.Language = value;
            propertyNameToSetterMap[Namespaces.DC   + "relation"]           = value => this.Relation = value;
        }

        #endregion
    }
}
