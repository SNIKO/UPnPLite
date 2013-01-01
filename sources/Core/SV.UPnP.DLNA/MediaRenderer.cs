
namespace SV.UPnP.DLNA
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using SV.UPnP.DLNA.Services.AvTransport;
    using SV.UPnP.DLNA.Services.ContentDirectory;

    /// <summary>
    ///     A device which renders content from Media Server.
    /// </summary>
    public class MediaRenderer : DLNADevice
    {
        #region Fields

        private readonly AvTransportService avTransportService;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DLNADevice" /> class.
        /// </summary>
        /// <param name="deviceInfo">
        ///     Defines parameters of the device.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="deviceInfo"/> is <c>null</c>.
        /// </exception>
        internal MediaRenderer(DeviceInfo deviceInfo)
            : base(deviceInfo)
        {
            this.avTransportService = new AvTransportService(deviceInfo.Services.FirstOrDefault(s => s.ServiceType.ToUpper().Contains("AVTRANSPORT")));
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
