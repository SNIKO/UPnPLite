
namespace SV.UPnP.DLNA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using SV.UPnP.DLNA.Services.AvTransport;
    using SV.UPnP.DLNA.Services.ContentDirectory;

    /// <summary>
    ///     A device which renders content from Media Server.
    /// </summary>
    public class MediaRenderer : UPnPDevice
    {
        #region Fields

        private readonly IAvTransportService avTransportService;

        private readonly Subject<MediaRendererState> stateChanges;

        private readonly Subject<TimeSpan> positionChanges;

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
            
            this.stateChanges = new Subject<MediaRendererState>();
            this.positionChanges = new Subject<TimeSpan>();
            this.avTransportService = avTransportService;

            Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)).Subscribe(
                async _ =>
                {
                    var info = await this.avTransportService.GetTransportInfoAsync(0);
                    var positionInfo = await this.avTransportService.GetPositionInfoAsync(0);
                    
                    this.State = ParseTransportState(info.State);
                    this.CurrentPosition = positionInfo.RelativeTimePosition;
                });
        }

        #endregion

        #region Events

        public IObservable<MediaRendererState> StateChanges
        {
            get
            {
                return this.stateChanges;
            }
        }

        public IObservable<TimeSpan> PositionChanges
        {
            get
            {
                return this.positionChanges;
            }
        }
        
        #endregion

        #region Properties

        public MediaRendererState State
        {
            get
            {
                return this.currentState;
            }
            private set
            {
                if (this.currentState != value)
                {
                    this.currentState = value;
                    this.stateChanges.OnNext(value);
                }
            }
        }

        public TimeSpan CurrentPosition
        {
            get
            {
                return this.currentPosition;
            }

            private set
            {
                if (this.currentPosition != value)
                {
                    this.currentPosition = value;
                    this.positionChanges.OnNext(value);
                }
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

        private MediaRendererState ParseTransportState(string transportState)
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
