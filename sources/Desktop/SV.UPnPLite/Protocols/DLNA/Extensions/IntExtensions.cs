
namespace SV.UPnPLite.Protocols.DLNA.Extensions
{
	using System;

	/// <summary>
	///     Defines extension methods for <see cref="int"/>.
	/// </summary>
	public static class IntExtensions
	{
		/// <summary>
		///     Converts an <paramref name="errorCode"/> to <see cref="MediaRendererError"/>.
		/// </summary>
		/// <param name="errorCode">
		///     An error code to convert.
		/// </param>
		/// <returns>
		///     A concrete <see cref="MediaRendererError"/> if <paramref name="errorCode"/> is defined; otherwise, <see cref="MediaRendererError.UnexpectedError"/>.
		/// </returns>
		public static MediaRendererError ToMediaRendererError(this int errorCode)
		{
			return Enum.IsDefined(typeof(MediaRendererError), errorCode)
					   ? (MediaRendererError)errorCode
					   : MediaRendererError.UnexpectedError;
		}

		/// <summary>
		///     Converts an <paramref name="errorCode"/> to <see cref="MediaServerError"/>.
		/// </summary>
		/// <param name="errorCode">
		///     An error code to convert.
		/// </param>
		/// <returns>
		///     A concrete <see cref="MediaServerError"/> if <paramref name="errorCode"/> is defined; otherwise, <see cref="MediaServerError.UnexpectedError"/>.
		/// </returns>
		public static MediaServerError ToMediaServerError(this int errorCode)
		{
			return Enum.IsDefined(typeof(MediaServerError), errorCode)
					   ? (MediaServerError)errorCode
					   : MediaServerError.UnexpectedError;
		}
	}
}
