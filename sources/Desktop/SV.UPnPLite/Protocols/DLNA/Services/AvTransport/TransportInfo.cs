
namespace SV.UPnPLite.Protocols.DLNA.Services.AvTransport
{
	/// <summary>
	///     Defines information associated with the current transport state.
	/// </summary>
	public class TransportInfo
	{
		/// <summary>
		///     Gets the conceptually top-level state of the <see cref="AvTransportService"/>.
		/// </summary>
		public string State { get; internal set; }

		/// <summary>
		///     Gets the current status of the <see cref="AvTransportService"/>.
		/// </summary>
		public string Status { get; internal set; }

		/// <summary>
		///     Gets the current playback speed of the <see cref="AvTransportService"/>.
		/// </summary>
		public string Speed { get; internal set; }
	}
}
