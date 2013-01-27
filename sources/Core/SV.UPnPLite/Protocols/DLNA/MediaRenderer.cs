
namespace SV.UPnPLite.Protocols.DLNA
{
    using SV.UPnPLite.Protocols.DLNA.Services.AvTransport;
    using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory;
    using SV.UPnPLite.Protocols.UPnP;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading.Tasks;

    /// <summary>
    ///     A device which renders content from Media Server.
    /// </summary>
    public class MediaRenderer : UPnPDevice
    {
        #region Fields

        private readonly IAvTransportService avTransportService;

        private readonly OnDemandObservable<MediaRendererState> stateChangesObservableSequence;

        private readonly OnDemandObservable<TimeSpan> positionChangesObservableSequence;

        private MediaRendererState currentState;

        private TimeSpan currentPosition;

        private readonly static Dictionary<string, MediaRendererState> statesMapper = new Dictionary<string, MediaRendererState>(StringComparer.OrdinalIgnoreCase)
                {
                    { TransportState.NoMediaPresent, MediaRendererState.NoMediaPresent },
                    { TransportState.PausedPlayback, MediaRendererState.Paused },
                    { TransportState.Playing, MediaRendererState.Playing },
                    { TransportState.Stopped, MediaRendererState.Stopped },
                    { TransportState.Transitioning, MediaRendererState.Buffering } 
                };

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

            this.positionChangesObservableSequence = new OnDemandObservable<TimeSpan>(o =>
                {
                    var subscription = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)).Subscribe(
                        async _ =>
                        {
                            var positionInfo = await this.avTransportService.GetPositionInfoAsync(0);

                            if (this.currentPosition != positionInfo.RelativeTimePosition)
                            {
                                this.currentPosition = positionInfo.RelativeTimePosition;
                                o.OnNext(positionInfo.RelativeTimePosition);
                            }
                        });

                    return subscription.Dispose;
                });

            this.stateChangesObservableSequence = new OnDemandObservable<MediaRendererState>(o =>
            {
                var subscription = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)).Subscribe(
                    async _ =>
                    {
                        var info = await this.avTransportService.GetTransportInfoAsync(0);
                        var state = ParseTransportState(info.State);

                        if (this.currentState != state)
                        {
                            this.currentState = state;
                            o.OnNext(ParseTransportState(info.State));
                        }
                    });

                return subscription.Dispose;
            });
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
        public async Task OpenAsync(MediaItem item)
        {
            var resource = this.SelectResourceForPlayback(item);

            await this.avTransportService.SetAvTransportURIAsync(0, resource.Uri, resource.Metadata);
        }

        /// <summary>
        ///     Requests the renderer to start playback.
        /// </summary>
        /// <returns>
        ///     An <see cref="Task"/> instance which notifies about completion the async operation.
        /// </returns>
        public async Task PlayAsync()
        {
            await this.avTransportService.PlayAsync(0, "1");
        }

        /// <summary>
        ///     Requests the renderer to stop playback.
        /// </summary>
        /// <returns>
        ///     An <see cref="Task"/> instance which notifies about completion the async operation.
        /// </returns>
        public async Task StopAsync()
        {
            await this.avTransportService.StopAsync(0);
        }

        /// <summary>
        ///     Requests the renderer to pause playback.
        /// </summary>
        /// <returns>
        ///     An <see cref="Task"/> instance which notifies about completion the async operation.
        /// </returns>
        public async Task PauseAsync()
        {
            await this.avTransportService.PauseAsync(0);
        }

        /// <summary>
        ///     Requests current playback position.
        /// </summary>
        /// <returns>
        ///     The current position in terms of time, from the beginning of the current track.
        /// </returns>
        public async Task<TimeSpan> GetCurrentPosition()
        {
            var info = await this.avTransportService.GetPositionInfoAsync(0);

            return info.RelativeTimePosition;
        }

        /// <summary>
        ///     Requests current playback state.
        /// </summary>
        /// <returns>
        ///     The conceptually top-level state of the MediaRenderer.
        /// </returns>
        public async Task<MediaRendererState> GetCurrentState()
        {
            var stateInfo = await this.avTransportService.GetTransportInfoAsync(0);

            return ParseTransportState(stateInfo.State);
        }

        private static MediaRendererState ParseTransportState(string transportState)
        {
            MediaRendererState result;

            if (statesMapper.TryGetValue(transportState, out result) == false)
            {
                result = MediaRendererState.Stopped;

                // TODO: Log about unexpected state
            }

            return result;
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
