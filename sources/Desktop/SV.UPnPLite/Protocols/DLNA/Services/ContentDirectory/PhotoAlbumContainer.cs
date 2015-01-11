
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
	using SV.UPnPLite.Logging;

	public class PhotoAlbumContainer : AlbumContainer
	{
		#region Constructors

		/// <summary>
		///		Initializes a new instance of the <see cref="PhotoAlbumContainer"/> class.
		/// </summary>
		/// <param name="logManager">
		///		The log manager to use for logging.
		///	</param>
		public PhotoAlbumContainer(ILogManager logManager = null)
			: base(logManager)
		{
		}

		#endregion
	}
}
