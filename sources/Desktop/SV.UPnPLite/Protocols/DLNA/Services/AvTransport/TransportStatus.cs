
namespace SV.UPnPLite.Protocols.DLNA.Services.AvTransport
{
	/// <summary>
	///     Defines statuses of the AVTransport which may change during it's work.
	/// </summary>
	public static class TransportStatus
	{
		/// <summary>
		///     The AVTransport is currently processing well.
		/// </summary>
		public const string Ok = "OK";

		/// <summary>
		///     An error occurred.
		/// </summary>
		/// <remarks>
		///     For example, some time after playback of a stream has been started (via SetAVTransportURI() and Play() actions), there may be network congestion or 
		///     server problems causing hiccups in the rendered media.
		/// </remarks>
		public const string ErrorOccurred = "ERROR_OCCURRED";
	}
}