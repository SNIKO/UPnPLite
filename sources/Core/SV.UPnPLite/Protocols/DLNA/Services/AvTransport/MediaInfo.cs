
namespace SV.UPnP.DLNA.Services.AvTransport
{
    using System;

    /// <summary>
    ///     Defines an information about media currently controlled by the AVTransport instance.
    /// </summary>
    public class MediaInfo
    {
        #region Properties

        /// <summary>
        ///     Gets a number of tracks currently controlled by the AVTransport instance. 
        /// </summary>
        public int NumberOfTracks { get; internal set; }

        /// <summary>
        ///    Gets the duration of the media specified by <see cref="CurrentUri"/>.
        /// </summary>
        public TimeSpan MediaDuration { get; internal set; }

        /// <summary>
        ///     Gets a reference, in the form of a URI, to the resource controlled by the AVTransport instance.
        /// </summary>
        public Uri CurrentUri { get; internal set; }

        /// <summary>
        ///     Gets the meta-data, in the form of a DIDL-Lite XML Fragment, associated with the resource specified by <see cref="CurrentUri"/>.
        /// </summary>
        public string CurrentUriMetadata { get; internal set; }

        /// <summary>
        ///     Gets the URI of the resource to be controlled when the playback of the current resource finishes.
        /// </summary>
        public Uri NextUri { get; internal set; }

        /// <summary>
        ///     Gets the meta-data, in the form of a DIDL-Lite XML Fragment, associated with the resource specified by <see cref="NextUri"/>.
        /// </summary>
        public string NextUriMetadata { get; internal set; }

        /// <summary>
        ///     Gets the storage medium of the resource specified by <see cref="CurrentUri"/>. 
        /// </summary>
        /// <remarks>
        ///     If no resource is specified, then the state variable is set to “NONE”. If <see cref="CurrentUri"/> refers to a resource received from the UPnP network, 
        ///     the state variable is set to “NETWORK”.
        /// </remarks>
        public string PlaybackMedium { get; internal set; }

        /// <summary>
        ///     Gets a storage medium where the resource specified by <see cref="CurrentUri"/> will be recorded when a Record action is issued.
        /// </summary>
        /// <remarks>
        ///     If no resource is specified, then the state variable is set to “NONE”.
        /// </remarks>
        public string RecordMedium { get; internal set; }

        /// <summary>
        ///     Gets the write protection status of the currently loaded media.
        /// </summary>
        public bool WriteStatus { get; internal set; }

        #endregion
    }
}
