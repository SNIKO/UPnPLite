
namespace SV.UPnPLite.Protocols.DLNA.Services.AvTransport
{
	using SV.UPnPLite.Extensions;
	using SV.UPnPLite.Logging;
	using SV.UPnPLite.Protocols.UPnP;
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Threading.Tasks;

	/// <summary>
	///     Enables control over the transport of audio and video streams. The service type defines a  ‘common’ model for A/V transport control suitable for a 
	///     generic user interface. It can be used to control a wide variety of disc, tape and solid-state based media devices such as CD players, VCRs and MP3 players. 
	/// </summary>
	public class AvTransportService : UPnPService, IAvTransportService
	{
		#region Constructors

		/// <summary>
		///     Initializes a new instanceId of the <see cref="AvTransportService" /> class.
		/// </summary>
		/// <param name="serviceType">
		///     A type of the service.
		/// </param>
		/// <param name="controlUri">
		///     An URL for sending commands to the service.
		/// </param>
		/// <param name="eventsUri">
		///     An URL for subscrinbing to service's events.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="serviceType"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
		///     <paramref name="controlUri"/> is <c>null</c> -OR-
		///     <paramref name="eventsUri"/> is <c>null</c>.
		/// </exception>
		public AvTransportService(string serviceType, Uri controlUri, Uri eventsUri)
			: base(serviceType, controlUri, eventsUri)
		{
		}

		/// <summary>
		///     Initializes a new instanceId of the <see cref="AvTransportService" /> class.
		/// </summary>
		/// <param name="serviceType">
		///     A type of the service.
		/// </param>
		/// <param name="controlUri">
		///     An URL for sending commands to the service.
		/// </param>
		/// <param name="eventsUri">
		///     An URL for subscrinbing to service's events.
		/// </param>
		/// <param name="logManager">
		///     The <see cref="ILogManager"/> to use for logging the debug information.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="serviceType"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
		///     <paramref name="controlUri"/> is <c>null</c> -OR-
		///     <paramref name="eventsUri"/> is <c>null</c> -OR-
		///     <paramref name="logManager"/> is <c>null</c>.
		/// </exception>
		public AvTransportService(string serviceType, Uri controlUri, Uri eventsUri, ILogManager logManager)
			: base(serviceType, controlUri, eventsUri, logManager)
		{
		}

		#endregion

		#region Methods

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
		///     An unexpected error occurred when executing request on service.
		/// </exception>
		public async Task SetAvTransportURIAsync(uint instanceId, string currentUri, string currentUriMetadata)
		{
			var arguments = new Dictionary<string, object>
                {
                    {"InstanceID", instanceId},
                    {"CurrentURI", currentUri},
                    {"CurrentURIMetaData", currentUriMetadata},
                };

			await this.InvokeActionAsync("SetAVTransportURI", arguments);
		}

		/// <summary>
		///      Specifies the URI of the resource to be controlled when the playback of the current resource (set earlier via <see cref="SetAvTransportURIAsync"/>) finishes.
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
		public async Task SetNextAvTransportURIAsync(uint instanceId, string nextUri, string nextUriMetadata)
		{
			var arguments = new Dictionary<string, object>
                                    {
                                        { "InstanceID", instanceId },
                                        { "NextURI", nextUri },
                                        { "NextURIMetaData", nextUriMetadata },
                                    };

			await this.InvokeActionAsync("SetNextAVTransportURI", arguments);
		}

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
		public async Task<RendererMediaInfo> GetMediaInfoAsync(uint instanceId)
		{
			var arguments = new Dictionary<string, object> { { "InstanceID", instanceId } };

			var response = await this.InvokeActionAsync("GetMediaInfo", arguments);

			var mediaInfo = new RendererMediaInfo
								{
									NumberOfTracks = response.GetValueOrDefault<int>("NrTracks"),
									MediaDuration = ParsingHelper.ParseTimeSpan(response.GetValueOrDefault<string>("MediaDuration")),
									CurrentUri = response.GetValueOrDefault<Uri>("CurrentURI"),
									CurrentUriMetadata = response.GetValueOrDefault<string>("CurrentURIMetaData"),
									NextUri = response.GetValueOrDefault<Uri>("NextURI"),
									NextUriMetadata = response.GetValueOrDefault<string>("NextURIMetaData"),
									PlaybackMedium = response.GetValueOrDefault<string>("PlayMedium"),
									RecordMedium = response.GetValueOrDefault<string>("RecordMedium"),
									WriteStatus = response.GetValueOrDefault<bool>("WriteStatus")
								};

			return mediaInfo;
		}

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
		public async Task<TransportInfo> GetTransportInfoAsync(uint instanceId)
		{
			var arguments = new Dictionary<string, object> { { "InstanceID", instanceId } };

			var response = await this.InvokeActionAsync("GetTransportInfo", arguments);

			var transportInfo = new TransportInfo
			{
				State = response.GetValueOrDefault<string>("CurrentTransportState"),
				Status = response.GetValueOrDefault<string>("CurrentTransportStatus"),
				Speed = response.GetValueOrDefault<string>("CurrentSpeed")
			};

			return transportInfo;
		}

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
		public async Task<PositionInfo> GetPositionInfoAsync(uint instanceId)
		{
			var arguments = new Dictionary<string, object> { { "InstanceID", instanceId } };

			var response = await this.InvokeActionAsync("GetPositionInfo", arguments);

			var positionInfo = new PositionInfo
			{
				Track = response.GetValueOrDefault<uint>("Track"),
				TrackDuration = ParsingHelper.ParseTimeSpan(response.GetValueOrDefault<string>("TrackDuration")),
				TrackMetaData = response.GetValueOrDefault<string>("TrackMetaData"),
				TrackUri = response.GetValueOrDefault<Uri>("TrackURI"),
				RelativeTimePosition = ParsingHelper.ParseTimeSpan(response.GetValueOrDefault<string>("RelTime")),
				AbsoluteTimePosition = ParsingHelper.ParseTimeSpan(response.GetValueOrDefault<string>("AbsTime")),
				RelativeCounterPosition = response.GetValueOrDefault<int>("RelCount"),
				AbsoluteCounterPosition = response.GetValueOrDefault<int>("AbsCount")
			};

			return positionInfo;
		}

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
		public async Task PlayAsync(uint instanceId, string speed)
		{
			var arguments = new Dictionary<string, object>
                                    {
                                        { "InstanceID", instanceId },
                                        { "Speed", speed },
                                    };

			try
			{
				await this.InvokeActionAsync("Play", arguments);
			}
			catch (FormatException)
			{
				// We don't expect result, so, we don't care if parsing error occurred
			}
		}

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
		public async Task PauseAsync(uint instanceId)
		{
			try
			{
				await this.InvokeActionAsync("Pause", new Dictionary<string, object> { { "InstanceID", instanceId } });
			}
			catch (FormatException)
			{
				// We don't expect result, so, we don't care if parsing error occurred
			}
		}

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
		public async Task StopAsync(uint instanceId)
		{
			try
			{
				await this.InvokeActionAsync("Stop", new Dictionary<string, object> { { "InstanceID", instanceId } });
			}
			catch (FormatException)
			{
				// We don't expect result, so, we don't care if parsing error occurred
			}
		}

		#endregion
	}
}
