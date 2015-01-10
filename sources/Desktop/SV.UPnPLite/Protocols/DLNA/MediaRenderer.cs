
namespace SV.UPnPLite.Protocols.DLNA
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Reactive.Linq;
	using System.Threading.Tasks;
	using SV.UPnPLite.Extensions;
	using SV.UPnPLite.Logging;
	using SV.UPnPLite.Protocols.DLNA.Extensions;
	using SV.UPnPLite.Protocols.DLNA.Services.AvTransport;
	using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory;
	using SV.UPnPLite.Protocols.UPnP;

	/// <summary>
	///     A device which renders content from Media Server.
	/// </summary>
	public class MediaRenderer : UPnPDevice
	{
		#region Fields

		private readonly IAvTransportService avTransportService;

		private readonly static Dictionary<string, MediaRendererState> statesMapper = new Dictionary<string, MediaRendererState>(StringComparer.OrdinalIgnoreCase)
                {
                    { TransportState.NoMediaPresent, MediaRendererState.NoMediaPresent },
                    { TransportState.PausedPlayback, MediaRendererState.Paused },
                    { TransportState.Playing, MediaRendererState.Playing },
                    { TransportState.Stopped, MediaRendererState.Stopped },
                    { TransportState.Transitioning, MediaRendererState.Buffering } 
                };

		private OnDemandObservable<MediaRendererState> stateChangesObservableSequence;

		private OnDemandObservable<TimeSpan> positionChangesObservableSequence;

		private MediaRendererState currentState;

		private TimeSpan currentPosition;

		#endregion

		#region Constructors

		/// <summary>
		///     Initializes a new instance of the <see cref="MediaRenderer"/> class.
		/// </summary>
		/// <param name="udn">
		///     A universally-unique identifier for the device.
		/// </param>
		/// <param name="avTransportService">
		///     A <see cref="IAvTransportService"/> to use for controlling the transport of media streams.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="udn"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
		///     <paramref name="avTransportService"/> is <c>null</c>.
		/// </exception>
		public MediaRenderer(string udn, IAvTransportService avTransportService)
			: base(udn)
		{
			avTransportService.EnsureNotNull("avTransportService");

			this.avTransportService = avTransportService;

			this.Initialize();
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="MediaRenderer"/> class.
		/// </summary>
		/// <param name="udn">
		///     A universally-unique identifier for the device.
		/// </param>
		/// <param name="avTransportService">
		///     A <see cref="IAvTransportService"/> to use for controlling the transport of media streams.
		/// </param>
		/// <param name="logManager">
		///     The <see cref="ILogManager"/> to use for logging the debug information.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="udn"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
		///     <paramref name="avTransportService"/> is <c>null</c> -OR-
		///     <paramref name="logManager"/> is <c>null</c>.
		/// </exception>
		public MediaRenderer(string udn, IAvTransportService avTransportService, ILogManager logManager)
			: base(udn, logManager)
		{
			avTransportService.EnsureNotNull("avTransportService");

			this.avTransportService = avTransportService;

			this.Initialize();
		}

		#endregion

		#region Events

		/// <summary>
		///     Gets an observable sequence which notifies about renderer state changes.
		/// </summary>
		public IObservable<MediaRendererState> StateChanges
		{
			get
			{
				return this.stateChangesObservableSequence;
			}
		}

		/// <summary>
		///     Gets an observable sequence which notifies about current playback position changes.
		/// </summary>
		public IObservable<TimeSpan> PositionChanges
		{
			get
			{
				return this.positionChangesObservableSequence;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///     Prepares the <paramref name="item"/> for playback on renderer.
		/// </summary>
		/// <param name="item">
		///     An item to play on renderer.
		/// </param>
		/// <returns>
		///     An <see cref="Task"/> instance which notifies about completion the async operation.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="item"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="MediaRendererException">
		///     An unexpected error occurred when executing request on device.
		/// </exception>
		public async Task OpenAsync(MediaItem item)
		{
			item.EnsureNotNull("item");

			var resource = this.SelectResourceForPlayback(item);

			try
			{
				await this.avTransportService.SetAvTransportURIAsync(0, resource.Uri, resource.Metadata);
			}
			catch (FormatException ex)
			{
				throw new MediaRendererException(this, MediaRendererError.UnexpectedError, "Received result is in a bad format", ex);
			}
			catch (UPnPServiceException ex)
			{
				throw new MediaRendererException(this, ex.ErrorCode.ToMediaRendererError(), "An error occurred when opening '{0}'".F(resource.Uri), ex);
			}
		}

		/// <summary>
		///     Requests the renderer to start playback.
		/// </summary>
		/// <returns>
		///     An <see cref="Task"/> instance which notifies about completion the async operation.
		/// </returns>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="MediaRendererException">
		///     An unexpected error occurred when executing request on device.
		/// </exception>
		public async Task PlayAsync()
		{
			try
			{
				await this.avTransportService.PlayAsync(0, "1");
			}
			catch (UPnPServiceException ex)
			{
				throw new MediaRendererException(this, ex.ErrorCode.ToMediaRendererError(), "An error occurred when requesting play current media", ex);
			}
		}

		/// <summary>
		///     Requests the renderer to stop playback.
		/// </summary>
		/// <returns>
		///     An <see cref="Task"/> instance which notifies about completion the async operation.
		/// </returns>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="MediaRendererException">
		///     An unexpected error occurred when executing request on device.
		/// </exception>
		public async Task StopAsync()
		{
			try
			{
				await this.avTransportService.StopAsync(0);
			}
			catch (UPnPServiceException ex)
			{
				throw new MediaRendererException(this, ex.ErrorCode.ToMediaRendererError(), "An error occurred when requesting stop current media", ex);
			}
		}

		/// <summary>
		///     Requests the renderer to pause playback.
		/// </summary>
		/// <returns>
		///     An <see cref="Task"/> instance which notifies about completion the async operation.
		/// </returns>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="MediaRendererException">
		///     An unexpected error occurred when executing request on device.
		/// </exception>
		public async Task PauseAsync()
		{
			try
			{
				await this.avTransportService.PauseAsync(0);
			}
			catch (UPnPServiceException ex)
			{
				throw new MediaRendererException(this, ex.ErrorCode.ToMediaRendererError(), "An error occurred when requesting pause current media", ex);
			}
		}

		/// <summary>
		///     Requests current playback position.
		/// </summary>
		/// <returns>
		///     The current position in terms of time, from the beginning of the current track.
		/// </returns>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="MediaRendererException">
		///     An unexpected error occurred when executing request on device.
		/// </exception>
		public async Task<TimeSpan> GetCurrentPosition()
		{
			try
			{
				var info = await this.avTransportService.GetPositionInfoAsync(0);

				return info.RelativeTimePosition;
			}
			catch (FormatException ex)
			{
				throw new MediaRendererException(this, MediaRendererError.UnexpectedError, "Received result is in a bad format", ex);
			}
			catch (UPnPServiceException ex)
			{
				throw new MediaRendererException(this, ex.ErrorCode.ToMediaRendererError(), "An error occurred when requesting current position", ex);
			}
		}

		/// <summary>
		///     Requests current playback state.
		/// </summary>
		/// <returns>
		///     The conceptually top-level state of the MediaRenderer.
		/// </returns>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="MediaRendererException">
		///     An unexpected error occurred when executing request on device.
		/// </exception>
		public async Task<MediaRendererState> GetCurrentState()
		{
			try
			{
				var stateInfo = await this.avTransportService.GetTransportInfoAsync(0);

				return ParseTransportState(stateInfo.State);
			}
			catch (FormatException ex)
			{
				throw new MediaRendererException(this, MediaRendererError.UnexpectedError, "Received result is in a bad format", ex);
			}
			catch (UPnPServiceException ex)
			{
				throw new MediaRendererException(this, ex.ErrorCode.ToMediaRendererError(), "An error occurred when requesting current state", ex);
			}
		}

		/// <summary>
		///     Requests current uri.
		/// </summary>
		/// <returns>
		///     The conceptually top-level state of the MediaRenderer.
		/// </returns>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="MediaRendererException">
		///     An unexpected error occurred when executing request on device.
		/// </exception>
		public async Task<Uri> GetCurrentUriAsync()
		{
			try
			{
				var mediaInfo = await this.avTransportService.GetMediaInfoAsync(0);

				return mediaInfo.CurrentUri;
			}
			catch (FormatException ex)
			{
				throw new MediaRendererException(this, MediaRendererError.UnexpectedError, "Received result is in a bad format", ex);
			}
			catch (UPnPServiceException ex)
			{
				throw new MediaRendererException(this, ex.ErrorCode.ToMediaRendererError(), "An error occurred when requesting current state", ex);
			}
		}

		private MediaRendererState ParseTransportState(string transportState)
		{
			MediaRendererState result;

			if (statesMapper.TryGetValue(transportState, out result) == false)
			{
				result = MediaRendererState.Stopped;

				this.logger.Instance().Warning("An enexpected transport state received", "Renderer".As(this.FriendlyName), "State".As(transportState));
			}

			return result;
		}

		private void Initialize()
		{
			this.positionChangesObservableSequence = new OnDemandObservable<TimeSpan>(o =>
			{
				var subscription = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)).Subscribe(
					async _ =>
					{
						try
						{
							var positionInfo = await this.avTransportService.GetPositionInfoAsync(0);

							if (this.currentPosition != positionInfo.RelativeTimePosition)
							{
								this.currentPosition = positionInfo.RelativeTimePosition;
								o.OnNext(positionInfo.RelativeTimePosition);
							}
						}
						catch (WebException ex)
						{
							this.logger.Instance().Warning(ex, "An error occurred when requesting position info", "Renderer".As(this.FriendlyName));
						}
						catch (FormatException ex)
						{
							this.logger.Instance().Warning(ex, "An error occurred when requesting position info", "Renderer".As(this.FriendlyName));
						}
						catch (UPnPServiceException ex)
						{
							this.logger.Instance().Warning(ex, "An error occurred when requesting position info", "Renderer".As(this.FriendlyName));
						}
					});

				return subscription.Dispose;
			});

			this.stateChangesObservableSequence = new OnDemandObservable<MediaRendererState>(o =>
			{
				var subscription = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)).Subscribe(
					async _ =>
					{
						try
						{
							var info = await this.avTransportService.GetTransportInfoAsync(0);
							var state = ParseTransportState(info.State);

							if (this.currentState != state)
							{
								this.currentState = state;
								o.OnNext(ParseTransportState(info.State));
							}
						}
						catch (WebException ex)
						{
							this.logger.Instance().Warning(ex, "An error occurred when requesting state info", "Renderer".As(this.FriendlyName));
						}
						catch (FormatException ex)
						{
							this.logger.Instance().Warning(ex, "An error occurred when requesting state info", "Renderer".As(this.FriendlyName));
						}
						catch (UPnPServiceException ex)
						{
							this.logger.Instance().Warning(ex, "An error occurred when requesting state info", "Renderer".As(this.FriendlyName));
						}
					});

				return subscription.Dispose;
			});
		}

		private MediaResource SelectResourceForPlayback(MediaItem mediaItem)
		{
			var resource = mediaItem.Resources.First();

			var imageItem = mediaItem as ImageItem;
			if (imageItem != null)
			{
				var maxWidth = 0.0;
				foreach (var mediaResource in imageItem.Resources)
				{
					if (mediaResource.Resolution.Width > maxWidth)
					{
						maxWidth = mediaResource.Resolution.Width;
						resource = mediaResource;
					}
				}
			}

			return resource;
		}

		#endregion
	}
}
