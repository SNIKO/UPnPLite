
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    /// <summary>
    ///     Defines the media item which can be played on MediaRenderer.
    /// </summary>
    public class MediaItem : MediaObject
    {
        #region Properties

        /// <summary>
        ///     Gets an id property of the item being referred to. 
        /// </summary>
        public string RefId { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes delegates which sets the an appropriate properties according to read parameters from XML. 
        /// </summary>
        /// <param name="propertyNameToSetterMap">
        ///     A map between name of the parameter in XML and delegate which sets an appropriate property on object.
        /// </param>
        protected override void InitializePropertySetters(Dictionary<XName, Action<string>> propertyNameToSetterMap)
        {
            base.InitializePropertySetters(propertyNameToSetterMap);

            propertyNameToSetterMap["refId"] = value => this.RefId = value;
        }

        #endregion
    }
}
