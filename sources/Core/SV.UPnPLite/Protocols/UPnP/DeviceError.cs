
namespace SV.UPnP
{
    public enum DeviceError
    {
        /// <summary>
        ///     Could be any of the following: not enough in args, too many in args, no in arg by that name, one or more in args are of the wrong data type.
        /// </summary>
        InvalidArgs = 402,

        /// <summary>
        ///     The immediate transition from current transport state to desired transport state is not supported by this device.
        /// </summary>
        TransitionNotAvailable = 701,

        /// <summary>
        ///     The transport is “hold locked”. (Some portable mobile devices have a small mechanical toggle switch called a “hold lock switch”. While this switch 
        ///     is ON, i.e., the transport is hold locked, the device is guarded against operations such as accidental power on when not in use, or interruption of play or 
        ///     record from accidental pressing of a front panel button or a GUI button.).
        /// </summary>
        TransportIsLocked = 705,

        /// <summary>
        ///     The media cannot be written (e.g., because of dust or a scratch).
        /// </summary>
        WriteError = 706,

        /// <summary>
        ///     The media is write-protected or is of a not writable type.
        /// </summary>
        MediaIsProtectedOrNotWritable = 707,

        /// <summary>
        ///     The storage format of the currently loaded media is not supported for recording by this device.
        /// </summary>
        FormatNotSupportedForRecording = 708,

        /// <summary>
        ///     There is no free space left on the loaded media.
        /// </summary>
        MediaIsFull = 709,

        /// <summary>
        ///     The specified instanceID is invalid for this AVTransport. 
        /// </summary>
        InvalidInstanceID = 718
    }
}
