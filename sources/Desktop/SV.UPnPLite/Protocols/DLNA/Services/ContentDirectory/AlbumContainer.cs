
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
	using SV.UPnPLite.Logging;
	using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory.Extensions;

	public class AlbumContainer : MediaContainer
	{
		#region Constructors

		/// <summary>
		///		Initializes a new instance of the <see cref="AlbumContainer"/> class.
		/// </summary>
		/// <param name="logManager">
		///		The log manager to use for logging.
		///	</param>
		public AlbumContainer(ILogManager logManager = null)
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

		/// <summary>
		///		Gets the entity responsible for making the album available.
		/// </summary>
		public string Publisher { get; private set; }

		/// <summary>
		///		Gets the entity responsible for making contributions to the album.
		/// </summary>
		public string Contributor { get; private set; }

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
			else if (key.Is("publisher"))
			{
				this.Publisher = value;
			}
			else if (key.Is("contributor"))
			{
				this.Contributor = value;
			}
			else
			{
				base.SetValue(key, value);
			}
		}

		#endregion
	}
}
