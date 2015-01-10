
namespace SV.UPnPLite.Protocols.DLNA
{
	using SV.UPnPLite.Protocols.UPnP;
	using System;

	/// <summary>
	///     Defines an error that occurred on a Media Server.
	/// </summary>
	public class MediaServerException : UPnPDeviceException<MediaServer>
	{
		#region Constructors

		/// <summary>
		///     Initializes a new instance of the <see cref="MediaServerException"/> class.
		/// </summary>
		/// <param name="server">
		///     An instance of the server which caused an error.
		/// </param>
		/// <param name="error">
		///     An error that occurred on <paramref name="server"/>
		/// </param>
		/// <param name="message">
		///     The message that describes an error.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="server"/> is <c>null</c>.
		/// </exception>
		public MediaServerException(MediaServer server, MediaServerError error, string message)
			: this(server, error, message, null)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="MediaServerException"/> class.
		/// </summary>
		/// <param name="server">
		///     An instance of the server which caused an error.
		/// </param>
		/// <param name="error">
		///     An error that occurred on <paramref name="server"/>
		/// </param>
		/// <param name="message">
		///     The message that describes an error.
		/// </param>
		/// <param name="innerException">
		///     The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. 
		/// </param>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="server"/> is <c>null</c>.
		/// </exception>
		public MediaServerException(MediaServer server, MediaServerError error, string message, Exception innerException)
			: base(server, message, innerException)
		{
			this.Error = error;
		}

		#endregion

		#region Properties

		/// <summary>
		///     Gets a concrete error.
		/// </summary>
		public MediaServerError Error { get; private set; }

		#endregion
	}
}
