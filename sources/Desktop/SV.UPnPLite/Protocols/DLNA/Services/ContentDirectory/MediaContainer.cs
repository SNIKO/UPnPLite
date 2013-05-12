
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
    using SV.UPnPLite.Extensions;
    using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory.Extensions;

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

        /// <summary>
        ///     Gets a revision of the currrent container.
        /// </summary>
        public uint Revision { get; internal set; }

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
            else if (key.Is("childCount"))
            {
                this.ChildCount = int.Parse(value);
            }
            else if (key.Is("searchable"))
            {
                this.Searchable = value.ToBool();
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
