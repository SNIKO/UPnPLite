
namespace SV.UPnP.DLNA.Services.ContentDirectory
{
    using System;

    /// <summary>
    ///     The base class for all media hosted by Media Server.
    /// </summary>
    public class MediaObject
    {
        #region Properties

        /// <summary>
        ///     An identifier for the object.  The value of each object id property must be unique with respect to the Content Directory.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        ///     Id property of object’s parent. The parentID of the Content Directory ‘root’ container must be set to the reserved value of  “-1”.  No other 
        ///     parentID attribute of any other Content Directory object may take this value. 
        /// </summary>
        public string ParentId { get; private set; }

        /// <summary>
        ///     Name of the object.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        ///     Primary content creator or owner of the object
        /// </summary>
        public string Creator { get; private set; }

        /// <summary>
        ///     Class of the object.
        /// </summary>
        public string Class { get; private set; }

        /// <summary>
        ///     When <c>true</c>, ability to modify a given object is confined to the Content Directory Service. Control point metadata write access is disabled.  
        /// </summary>
        public bool Restricted { get; private set; }

        /// <summary>
        ///     Resource, typically a media file, associated with the object. Values must be properly escaped URIs as described in.
        /// </summary>
        public Uri Resource { get; private set; }

        #endregion
    }
}
