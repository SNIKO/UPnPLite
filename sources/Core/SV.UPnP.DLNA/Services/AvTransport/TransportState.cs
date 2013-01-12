
namespace SV.UPnP.DLNA.Services.AvTransport
{
    /// <summary>
    ///     Defines the conceptually top-level state of the transport.
    /// </summary>
    public static class TransportState
    {
        /// <summary>
        ///     The media is currently stopped.
        /// </summary>
        public const string Stopped = "STOPPED";

        /// <summary>
        ///     The media is currently playing.
        /// </summary>
        public const string Playing = "PLAYING";

        /// <summary>
        ///     The media is currently buffering.
        /// </summary>
        public const string Transitioning = "TRANSITIONING";

        /// <summary>
        ///     The media playback is currently paused. 
        /// </summary>
        /// <remarks>
        ///     The state is different from the <see cref="PausedRecording"/> state in the sense that in case the media contains video, it indicates output of a still image.
        /// </remarks>
        public const string PausedPlayback = "PAUSED_PLAYBACK";

        /// <summary>
        ///     The media recording is currently paused. 
        /// </summary>
        /// <remarks>
        ///     The state is different from the <see cref="Stopped"/> state in the sense that the transport is already prepared for recording and may respond faster or more accurate.
        /// </remarks>
        public const string PausedRecording = "PAUSED_RECORDING";

        /// <summary>
        ///     The media is currently recording.
        /// </summary>
        public const string Recording = "RECORDING";

        /// <summary>
        ///     No media is specified.
        /// </summary>
        public const string NoMediaPresent = "NO_MEDIA_PRESENT";
    }
}
