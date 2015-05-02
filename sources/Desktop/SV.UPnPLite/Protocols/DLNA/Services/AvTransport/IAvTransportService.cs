
namespace SV.UPnPLite.Protocols.DLNA.Services.AvTransport
{
	using SV.UPnPLite.Protocols.UPnP;
	using System;
	using System.Net;
	using System.Threading.Tasks;

	/// <summary>
	///     Defines members for controling the transport of audio and video streams. The service type defines a ‘common’ model for A/V transport control suitable for a 
	///     generic user interface. It can be used to control a wide variety of disc, tape and solid-state based media devices such as CD players, VCRs and MP3 players. 
	/// </summary>
	public interface IAvTransportService
	{
		/// <summary>
		///      Specifies the URI of the resource to be controlled by the specified AVTransport instance.
		/// </summary>
		/// <param name="instanceId">
		///      Identifies the virtual instanceId of the AVTransport service to which the action applies.
		/// </param>
		/// <param name="currentUri">
		///     The URI to the resource to control.
		/// </param>
		/// <param name="currentUriMetadata">
		///     The metadata, in the form of a DIDL-Lite XML fragment, associated with the resource pointed by <paramref name="currentUri"/>.
		/// </param>
		/// <returns>
		///     A <see cref="Task"/> instance which could be use for waiting an operation to complete.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="currentUri"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="FormatException">
		///     Received result is in a bad format.
		/// </exception>
		/// <exception cref="UPnPServiceException">
		///     An internal service error occurred when executing request.
		/// </exception>
		Task SetAvTransportURIAsync(uint instanceId, string currentUri, string currentUriMetadata);

		/// <summary>
		///      Specifies the URI of the resource to be controlled when the playback of the current resource (set earlier via <see cref="AvTransportService.SetAvTransportURIAsync"/>) finishes.
		/// </summary>
		/// <param name="instanceId">
		///      Identifies the virtual instanceId of the AVTransport service to which the action applies.
		/// </param>
		/// <param name="nextUri">
		///     The URI to the next resource to control.
		/// </param>
		/// <param name="nextUriMetadata">
		///     The metadata, in the form of a DIDL-Lite XML fragment, associated with the resource pointed by <paramref name="nextUri"/>.
		/// </param>
		/// <returns>
		///     A <see cref="Task"/> instance which could be use for waiting an operation to complete.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="nextUri"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="FormatException">
		///     Received result is in a bad format.
		/// </exception>
		/// <exception cref="UPnPServiceException">
		///     An internal service error occurred when executing request.
		/// </exception>
		Task SetNextAvTransportURIAsync(uint instanceId, string nextUri, string nextUriMetadata);

		/// <summary>
		///       Returns information associated with the current media of the specified instance.
		/// </summary>
		/// <param name="instanceId">
		///      Identifies the virtual instanceId of the AVTransport service to which the action applies.
		/// </param>
		/// <returns>
		///     An instance of <see cref="RendererMediaInfo"/> which defines information about currently current media.
		/// </returns>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="FormatException">
		///     Received result is in a bad format.
		/// </exception>
		/// <exception cref="UPnPServiceException">
		///     An internal service error occurred when executing request.
		/// </exception>
		Task<RendererMediaInfo> GetMediaInfoAsync(uint instanceId);

		/// <summary>
		///       Returns information associated with the current transport state of the specified instance.
		/// </summary>
		/// <param name="instanceId">
		///      Identifies the virtual instanceId of the AVTransport service to which the action applies.
		/// </param>
		/// <returns>
		///     An instance of <see cref="TransportInfo"/> which defines information about specified AvTransport instance.
		/// </returns>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="FormatException">
		///     Received result is in a bad format.
		/// </exception>
		/// <exception cref="UPnPServiceException">
		///     An internal service error occurred when executing request.
		/// </exception>
		Task<TransportInfo> GetTransportInfoAsync(uint instanceId);

		/// <summary>
		///       Returns information associated with the current position of the transport of the specified instance.
		/// </summary>
		/// <param name="instanceId">
		///      Identifies the virtual instanceId of the AVTransport service to which the action applies.
		/// </param>
		/// <returns>
		///     An instance of <see cref="PositionInfo"/> which defines information about playback position of the specified AvTransport instance.
		/// </returns>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="FormatException">
		///     Received result is in a bad format.
		/// </exception>
		/// <exception cref="UPnPServiceException">
		///     An internal service error occurred when executing request.
		/// </exception>
		Task<PositionInfo> GetPositionInfoAsync(uint instanceId);

		/// <summary>
		///     Starts playing the resource of the specified instanceId, at the specified speed, starting at the current position, according to the current play mode.
		/// </summary>
		/// <param name="instanceId">
		///      Identifies the virtual instanceId of the AVTransport service to which the action applies.
		/// </param>
		/// <param name="speed">
		///     Indicates the speed relative to normal speed. Example values are ‘1’, ‘1/2’, ‘2’, ‘-1’, ‘1/10’, etc.
		/// </param>
		/// <returns>
		///     A <see cref="Task"/> instance which could be use for waiting an operation to complete.
		/// </returns>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="UPnPServiceException">
		///     An internal service error occurred when executing request.
		/// </exception>
		Task PlayAsync(uint instanceId, string speed);

		/// <summary>
		///      Halts the progression of the resource that is associated with the specified instance.
		/// </summary>
		/// <param name="instanceId">
		///      Identifies the virtual instanceId of the AVTransport service to which the action applies.
		/// </param>
		/// <returns>
		///     A <see cref="Task"/> instance which could be use for waiting an operation to complete.
		/// </returns>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="UPnPServiceException">
		///     An internal service error occurred when executing request.
		/// </exception>
		Task PauseAsync(uint instanceId);

		/// <summary>
		///      Stops the progression of the current resource that is associated with the specified instance.
		/// </summary>
		/// <param name="instanceId">
		///      Identifies the virtual instanceId of the AVTransport service to which the action applies.
		/// </param>
		/// <returns>
		///     A <see cref="Task"/> instance which could be use for waiting an operation to complete.
		/// </returns>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="UPnPServiceException">
		///     An internal service error occurred when executing request.
		/// </exception>
		Task StopAsync(uint instanceId);
	}
}