
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
	using System;
	using SV.UPnPLite.Logging;
	using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory.Extensions;

    /// <summary>
    ///      Represents a discrete piece of audio that should be interpreted as music.
    /// </summary>
    public class MusicTrack : AudioItem
    {
        #region Properties

        /// <summary>
        ///     Gets a name of an artist.
        /// </summary>
        public string Artist { get; internal set; }

        /// <summary>
        ///     Gets a title of the album to which the item belongs.
        /// </summary>
        public string Album { get; internal set; }

        /// <summary>
        ///     Gets a name of the primary content creator or owner.
        /// </summary>
        public string Contributor { get; internal set; }

        /// <summary>
        ///     Gets a date when the item was created.
        /// </summary>
        public DateTime Date { get; internal set; }

        /// <summary>
        ///     Gets a URL to a album cover.
        /// </summary>
        public string AlbumArtUri { get; internal set; }

		/// <summary>
		///		Gets the original track number on an audio CD or other medium.
		/// </summary>		
		public int TrackNumber { get; internal set; }

        #endregion

		#region Constructors

		public MusicTrack(ILogManager logManager)
			: base(logManager)
		{
		}

		#endregion

        #region Methods

        /// <summary>
		///     Sets a value read from an object's metadata XML.
        /// </summary>
        /// <param name="key">
        ///     The key of the property read from XML.
        /// </param>
        /// <param name="value">
        ///     The value of the property read from XML.
        /// </param>
        protected override void SetValue(string key, string value)
        {
            if (key.Is("artist"))
            {
                this.Artist = value;
            }
            else if (key.Is("album"))
            {
                this.Album = value;
            }
            else if (key.Is("albumArtURI"))
            {
                this.AlbumArtUri = value;
            }
            else if (key.Is("contributor"))
            {
                this.Contributor = value;
            }
            else if (key.Is("date"))
            {
                this.Date = ParsingHelper.ParseDate(value);
            }
			else if(key.Is("originalTrackNumber"))
			{
				this.TrackNumber = int.Parse(value);
			}
            else
            {
				base.SetValue(key, value);
            }
        }

        #endregion
    }
}
