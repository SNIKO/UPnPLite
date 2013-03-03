
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
    using SV.UPnPLite.Extensions;
    using System;
    using System.Xml.Linq;

    /// <summary>
    ///     Defines a media object which contains other media objects.
    /// </summary>
    public class MediaContainer : MediaObject
    {
        #region Properties

        /// <summary>
        ///     Gets a child count for the object.
        /// </summary>
        public int ChildCount { get; internal set; }

        /// <summary>
        ///     Gets a flag indicating whether the container has ability to perform search;
        /// </summary>
        public bool Searchable { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes delegates which sets the an appropriate properties according to read parameters from XML. 
        /// </summary>
        /// <param name="propertyNameToSetterMap">
        ///     A map between name of the parameter in XML and delegate which sets an appropriate property on object.
        /// </param>
        protected override void InitializePropertySetters(System.Collections.Generic.Dictionary<string, Action<string>> propertyNameToSetterMap)
        {
            base.InitializePropertySetters(propertyNameToSetterMap);

            propertyNameToSetterMap["childCount"] = value => this.ChildCount = int.Parse(value);
            propertyNameToSetterMap["searchable"] = value => this.Searchable = value.ToBool();
        }

        #endregion
    }
}
