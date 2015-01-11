
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
	using SV.UPnPLite.Logging;
	using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory.Extensions;

	public class MusicAlbumContainer : AlbumContainer
	{
		#region Constructors

		/// <summary>
		///		Initializes a new instance of the <see cref="MusicAlbumContainer"/> class.
		/// </summary>
		/// <param name="logManager">
		///		The log manager to use for logging.
		///	</param>
		public MusicAlbumContainer(ILogManager logManager = null)
			: base(logManager)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		///		Gets the artist of the album.
		/// </summary>
		public string Artist { get; private set; }

		/// <summary>
		///		Gets the genre of the album.
		/// </summary>
		public string Genre { get; private set; }

		/// <summary>
		///		Gets the producer of the album.
		/// </summary>
		public string Producer { get; private set; }

		/// <summary>
		///		Gets the reference to album art.
		/// </summary>
		public string AlbumArtURI { get; private set; }

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
			if (key.Is("artist"))
			{
				this.Artist = value;
			}
			else if (key.Is("genre"))
			{
				this.Genre = value;
			}
			else if (key.Is("producer"))
			{
				this.Producer = value;
			}
			else if (key.Is("albumArtURI"))
			{
				this.AlbumArtURI = value;
			}
			else
			{
				base.SetValue(key, value);
			}
		}

		#endregion
	}
}
