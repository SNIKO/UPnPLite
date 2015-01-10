
namespace SV.UPnPLite.Protocols.DLNA
{
	using System;
	using SV.UPnPLite.Protocols.UPnP;

	/// <summary>
	///     Defines an error that occurred on a Media Renderer.
	/// </summary>
	public class MediaRendererException : UPnPDeviceException<MediaRenderer>
	{
		#region Constructors

		/// <summary>
		///     Initializes a new instance of the <see cref="MediaRendererException"/> class.
		/// </summary>
		/// <param name="renderer">
		///     An instance of the renderer which caused an error.
		/// </param>
		/// <param name="error">
		///     An error that occurred on <paramref name="renderer"/>
		/// </param>
		/// <param name="message">
		///     The message that describes an error.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="renderer"/> is <c>null</c>.
		/// </exception>
		public MediaRendererException(MediaRenderer renderer, MediaRendererError error, string message)
			: this(renderer, error, message, null)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="MediaRendererException"/> class.
		/// </summary>
		/// <param name="renderer">
		///     An instance of the renderer which caused an error.
		/// </param>
		/// <param name="error">
		///     An error that occurred on <paramref name="renderer"/>
		/// </param>
		/// <param name="message">
		///     The message that describes an error.
		/// </param>
		/// <param name="innerException">
		///     The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. 
		/// </param>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="renderer"/> is <c>null</c>.
		/// </exception>
		public MediaRendererException(MediaRenderer renderer, MediaRendererError error, string message, Exception innerException)
			: base(renderer, message, innerException)
		{
			this.Error = error;
		}

		#endregion

		#region Properties

		/// <summary>
		///     Gets a concrete error.
		/// </summary>
		public MediaRendererError Error { get; private set; }

		#endregion
	}
}
