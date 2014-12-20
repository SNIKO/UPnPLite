
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
	using SV.UPnPLite.Extensions;
	using SV.UPnPLite.Logging;
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

		#region Constructors

		/// <summary>
		///		Initializes a new instance of the <see cref="MediaContainer"/> class.
		/// </summary>
		/// <param name="logManager">
		///		The log manager to use for logging.
		///	</param>
		public MediaContainer(ILogManager logManager)
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
            if (key.Is("childCount"))
            {
                this.ChildCount = int.Parse(value);
            }
            else if (key.Is("searchable"))
            {
                this.Searchable = value.ToBool();
            }
            else
            {
                base.SetValue(key, value);
            }
        }

        #endregion
    }
}
