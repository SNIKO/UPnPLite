
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
    using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory.Extensions;
    using System;

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
        /// <returns>
        ///     <c>true</c>, if the value was set; otherwise, <c>false</c>.
        /// </returns>
        protected override bool TrySetValue(string key, string value)
        {
            if (base.TrySetValue(key, value))
            {
                // The value is set by base object
            }
            else if (key.Is("artist"))
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
                this.Date = ParseDate(value);
            }
            else
            {
                return false;
            }

            return true;
        }

        private static DateTime ParseDate(string date)
        {
            DateTime result;

            var parts = date.Split('-');
            if (parts.Length == 3)
            {
                int year;
                int month;
                int day;

                if (int.TryParse(parts[0], out year) && int.TryParse(parts[1], out month) &&
                    int.TryParse(parts[2], out day))
                {
                    result = new DateTime(year, month, day);
                }
                else
                {
                    result = default(DateTime);
                }
            }
            else
            {
                result = default(DateTime);
            }

            return result;
        }

        #endregion
    }
}
