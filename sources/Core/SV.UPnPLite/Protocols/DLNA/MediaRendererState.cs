
namespace SV.UPnPLite.Protocols.DLNA
{
    /// <summary>
    ///     Defines the playback state of the renderer.
    /// </summary>
    public enum MediaRendererState
    {
        /// <summary>
        ///     The media is currently stopped.
        /// </summary>
        Stopped,

        /// <summary>
        ///     The media is currently playing.
        /// </summary>
        Playing,

        /// <summary>
        ///     The media is currently buffering.
        /// </summary>
        Buffering,

        /// <summary>
        ///     The media playback is currently paused. 
        /// </summary>
        Paused,

        /// <summary>
        ///     No media is specified.
        /// </summary>
        NoMediaPresent
    }
}
