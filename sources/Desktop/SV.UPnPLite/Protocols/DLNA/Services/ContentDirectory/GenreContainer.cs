
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
	using SV.UPnPLite.Logging;
	using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory.Extensions;

	public class GenreContainer : MediaContainer
	{
		#region Constructors

		/// <summary>
		///		Initializes a new instance of the <see cref="GenreContainer"/> class.
		/// </summary>
		/// <param name="logManager">
		///		The log manager to use for logging.
		///	</param>
		public GenreContainer(ILogManager logManager = null)
			: base(logManager)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		///		Gets the short description of the album.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		///		Gets the long description of the album.
		/// </summary>
		public string LongDescription { get; private set; }

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
			if (key.Is("description"))
			{
				this.Description = value;
			}
			else if (key.Is("longDescription"))
			{
				this.LongDescription = value;
			}
			else
			{
				base.SetValue(key, value);
			}
		}

		#endregion
	}
}
