
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
    using System.Collections.Generic;

    /// <summary>
    ///     Defines result for a <see cref="ContentDirectoryService.BrowseAsync"/> action.
    /// </summary>
    public class BrowseResult
    {
        #region Properties

        /// <summary>
        ///     Gets the list of found media items.
        /// </summary>
        public IEnumerable<MediaObject> Result { get; internal set; }

        /// <summary>
        ///     Gets number of objects returned in this result. If <see cref="BrowseFlag.BrowseMetadata"/> is specified, then returns 1.
        /// </summary>
        public int NumberReturned { get; internal set; }

        /// <summary>
        ///  Gets total number of objects in the container. If <see cref="BrowseFlag.BrowseMetadata"/> is specified, then returns 1.
        /// </summary>
        public int TotalMatches { get; internal set; }

        /// <summary>
        ///     Gets the UpdateId property of the container being described if a container is specified in ObjectId.
        /// </summary>
        /// <remarks>
        ///     If the control point has an UpdateID for the container that is not equal to the UpdateID last returned, then the control point should refresh all 
        ///     its state relative to that container. If the ObjectID is zero, then the UpdateID returned is SystemUpdateID.
        /// </remarks>
        public uint UpdateId { get; internal set; }

        #endregion
    }
}
