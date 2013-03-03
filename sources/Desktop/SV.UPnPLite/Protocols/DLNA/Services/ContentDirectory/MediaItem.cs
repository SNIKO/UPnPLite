
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
    using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory.Extensions;

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
            else if (key.Is("refId"))
            {
                this.RefId = value;
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
