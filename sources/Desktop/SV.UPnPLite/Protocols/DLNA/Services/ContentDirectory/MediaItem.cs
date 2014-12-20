
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
	using SV.UPnPLite.Logging;
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

		#region Constructors

		/// <summary>
		///		Initializes a new instance of the <see cref="MediaItem"/> class.
		/// </summary>
		/// <param name="logManager">
		///		The log manager to use for logging.
		///	</param>
		public MediaItem(ILogManager logManager = null)
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
            if (key.Is("refId"))
            {
                this.RefId = value;
            }
			else
			{
				base.SetValue(key, value);
			}
        }

        #endregion
    }
}
