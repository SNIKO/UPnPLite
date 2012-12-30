
namespace SV.UPnP.DLNA.Services.ContentDirectory
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Windows.Foundation;

    /// <summary>
    ///     Defines a media resource -  some type of a binary asset, such as photo, song, video, etc.
    ///  </summary>
    public class MediaResource
    {
        #region Fields

        private Dictionary<XName, Action<string>> propertySetters;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets size in bytes of the resource.
        /// </summary>
        public ulong Size { get; internal set; }

        /// <summary>
        ///     Gets a time duration of the playback of the resource, at normal speed.
        /// </summary>
        public TimeSpan Duration { get; internal set; }

        /// <summary>
        ///     Gets a bitrate in bytes/seconds of the encoding of the resource.
        /// </summary>
        public uint Bitrate { get; internal set; }

        /// <summary>
        ///     Gets a sample frequency of the audio in HZ.
        /// </summary>
        public uint SampleFrequency { get; internal set; }

        /// <summary>
        ///     Gets a number of bits per second of the resource.
        /// </summary>
        public uint BitsPerSample { get; internal set; }

        /// <summary>
        ///     Gets a number of audio channels, e.g., 1 for mono, 2 for stereo, 6 for Dolby Surround.
        /// </summary>
        public uint NumberOfAudioChannels { get; internal set; }

        /// <summary>
        ///     Gets resolution of the resource in pixels (typically image item or video item).
        /// </summary>
        public Size Resolution { get; internal set; }

        /// <summary>
        ///     Gets a color depth of the resource.
        /// </summary>
        public uint ColorDepth { get; internal set; }

        /// <summary>
        ///     Gets a string that identifies the recommended HTTP protocol for transmitting  the resource. If not present, then the content has not yet been fully 
        ///     imported by CDS and is not yet accessible for playback purpose.
        /// </summary>
        public string ProtocolInfo { get; internal set; }

        /// <summary>
        ///     Gets some identification of a protection system used for the resource.
        /// </summary>
        public string Protection { get; internal set; }

        /// <summary>
        ///     Gets a URI via which the resource can be imported to the CDS via ImportResource() or HTTP POST.
        /// </summary>
        public Uri ImportUri { get; internal set; }

        /// <summary>
        ///     Gets a URI via which the resource can be accessed.
        /// </summary>
        public Uri Uri { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Deserilizes the resource from a DIDL-Lite XML.
        /// </summary>
        /// <param name="resourceXml">
        ///     A resource XML created according to DIDL-Lite schema.
        /// </param>
        public MediaResource Deserialize(string resourceXml)
        {
            this.EnsurePropertySettersInititalized();

            var resourceElement = XElement.Parse(resourceXml);

            foreach (var attribute in resourceElement.Attributes())
            {
                Action<string> propertySetter;
                if (this.propertySetters.TryGetValue(attribute.Name, out propertySetter))
                {
                    propertySetter(attribute.Value);
                }
            }

            this.Uri = new Uri(resourceElement.Value);

            return this;
        }

        /// <summary>
        ///     Initializes delegates which sets the an appropriate properties according to read parameters from XML. 
        /// </summary>
        /// <param name="propertyNameToSetterMap">
        ///     A map between name of the parameter in XML and delegate which sets an appropriate property on object.
        /// </param>
        protected virtual void InitializePropertySetters(Dictionary<XName, Action<string>> propertyNameToSetterMap)
        {
            propertyNameToSetterMap["size"]             = value => this.Size = uint.Parse(value);
            propertyNameToSetterMap["duration"]         = value => this.Duration = ParseTimeSpan(value);
            propertyNameToSetterMap["bitrate"]          = value => this.Bitrate = uint.Parse(value);
            propertyNameToSetterMap["sampleFrequency"]  = value => this.SampleFrequency = uint.Parse(value);
            propertyNameToSetterMap["bitsPerSample"]    = value => this.BitsPerSample = uint.Parse(value);
            propertyNameToSetterMap["nrAudioChannels"]  = value => this.NumberOfAudioChannels = uint.Parse(value);
            propertyNameToSetterMap["resolution"]       = value => ParseResolution(value);
            propertyNameToSetterMap["colorDepth"]       = value => this.ColorDepth = uint.Parse(value);
            propertyNameToSetterMap["protocolInfo"]     = value => this.ProtocolInfo = value;
            propertyNameToSetterMap["protection"]       = value => this.Protection = value;
            propertyNameToSetterMap["importUri"]        = value => this.ImportUri = new Uri(value);
        }

        /// <summary>
        ///     Parses the time span string in format of DIDL-Lite into <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="timeSpanString">
        ///     The string to parse.
        /// </param>
        /// <returns>
        ///     A <see cref="TimeSpan"/> instance.
        /// </returns>
        /// <remarks>
        ///     The form of the duration string is: H+:MM:SS[.F+], or H+:MM:SS[.F0/F1] where : H+ :  number of digits (including no digits) to indicate elapsed hours, MM : exactly 2 
        ///     digits to indicate minutes (00 to 59), SS : exactly 2 digits to indicate seconds (00 to 59), F+ : any number of digits (including no digits) to indicate fractions of 
        ///     seconds, F0/F1 : a fraction, with F0 and F1 at least one digit long, and F0 less than F1. The string may be preceded by an optional + or – sign, and the decimal point 
        ///     itself may be omitted if there are no fractional second digits.  
        /// </remarks>
        private static TimeSpan ParseTimeSpan(string timeSpanString)
        {
            var splitted = timeSpanString.Split(':');
            var splittedSeconds = splitted[2].Split('.');

            var hours = int.Parse(splitted[0]);
            var minutes = int.Parse(splitted[1]);
            var seconds = int.Parse(splittedSeconds[0]);
            var milliseconds = splittedSeconds.Length == 2 ? int.Parse(splittedSeconds[1]) : 0;

            return new TimeSpan(0, hours, minutes, seconds, milliseconds);
        }

        /// <summary>
        ///     Parses the resolution string into <see cref="Size"/>.
        /// </summary>
        /// <param name="resolutionString">
        ///     The string to parse.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="Size"/>
        /// </returns>
        /// <remarks>
        ///     String pattern is of the form: [0-9]+x[0-9]+ (one or more digits,'x', followed by one or more digits).
        /// </remarks>
        private static Size ParseResolution(string resolutionString)
        {
            var splitted = resolutionString.Split('x');

            var width = int.Parse(splitted[0]);
            var height = int.Parse(splitted[1]);

            return new Size(width, height);
        }

        private void EnsurePropertySettersInititalized()
        {
            if (this.propertySetters == null)
            {
                this.propertySetters = new Dictionary<XName, Action<string>>(XNameComparer.OrdinalIgnoreCase);

                this.InitializePropertySetters(this.propertySetters);
            }
        }


        #endregion
    }
}
