
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
    using System;
    using System.Collections.Generic;

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

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes delegates which sets the an appropriate properties according to read parameters from XML. 
        /// </summary>
        /// <param name="propertyNameToSetterMap">
        ///     A map between name of the parameter in XML and delegate which sets an appropriate property on object.
        /// </param>
        protected override void InitializePropertySetters(Dictionary<System.Xml.Linq.XName, Action<string>> propertyNameToSetterMap)
        {
            base.InitializePropertySetters(propertyNameToSetterMap);

            propertyNameToSetterMap[Namespaces.UPnP + "artist"]         = value => this.Artist = value;
            propertyNameToSetterMap[Namespaces.UPnP + "album"]          = value => this.Album = value;
            propertyNameToSetterMap[Namespaces.DC   + "contributor"]    = value => this.Contributor = value;
            propertyNameToSetterMap[Namespaces.DC   + "date"]           = value => this.Date = ParseDate(value);
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
