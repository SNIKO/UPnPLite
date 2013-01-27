
namespace SV.UPnPLite.Protocols.DLNA.Services.AvTransport
{
    using System;

    /// <summary>
    ///     Defines an information about current playback position of the AvTransport instance.
    /// </summary>
    public class PositionInfo
    {
        /// <summary>
        ///     Gets the sequence number of the currently selected track, starting at value ‘1’, up to and including NumberOfTracks. If NumberOfTracks is 0, then Track will be 0.
        /// </summary>
        public uint Track { get; internal set; }

        /// <summary>
        ///     Gets a duration of the current track.
        /// </summary>
        public TimeSpan TrackDuration { get; internal set; }

        /// <summary>
        ///     Gets a metadata in a format of DIDL-Lite XML of the current track.
        /// </summary>
        public string TrackMetaData { get; internal set; }

        /// <summary>
        ///     Gets a Uri of the the current track.
        /// </summary>
        public Uri TrackUri { get; internal set; }

        /// <summary>
        ///     Gets the current position in terms of time, from the beginning of the current track.
        /// </summary>
        public TimeSpan RelativeTimePosition { get; internal set; }

        /// <summary>
        ///     Gets the current position in terms of time, from the beginning of the media.
        /// </summary>
        public TimeSpan AbsoluteTimePosition { get; internal set; }

        /// <summary>
        ///     Gets the current position in terms of dimensionless counter, from the beginning of the current track.
        /// </summary>
        public int RelativeCounterPosition { get; internal set; }

        /// <summary>
        ///     Gets the current position in terms of dimensionless counter, from the beginning of the media.
        /// </summary>
        public int AbsoluteCounterPosition { get; internal set; }
    }
}
