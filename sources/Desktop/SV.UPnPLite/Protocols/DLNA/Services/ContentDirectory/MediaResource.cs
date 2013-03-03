
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    
    /// <summary>
    ///     Defines a media resource -  some type of a binary asset, such as photo, song, video, etc.
    ///  </summary>
    public class MediaResource
    {
        #region Fields

        private static readonly XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
        {
            IgnoreComments = true,
            IgnoreWhitespace = true,
            CloseInput = true,
            IgnoreProcessingInstructions = true
        };

        private Dictionary<string, Action<string>> propertySetters;

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
        public string ImportUri { get; internal set; }

        /// <summary>
        ///     Gets a URI via which the resource can be accessed.
        /// </summary>
        public string Uri { get; internal set; }

        /// <summary>
        ///     Gets a metadata associated with the resource.
        /// </summary>
        public string Metadata { get; internal set; }

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

            using (var reader = XmlReader.Create(new StringReader(resourceXml), xmlReaderSettings))
            {
                reader.Read();

                while (reader.MoveToNextAttribute())
                {
                    Action<string> propertySetter;
                    if (this.propertySetters.TryGetValue(reader.LocalName, out propertySetter))
                    {
                        propertySetter(reader.Value);
                    }
                }

                reader.MoveToElement();

                this.Uri = reader.ReadElementContentAsString();
            }

            this.Metadata = resourceXml;

            return this;
        }

        /// <summary>
        ///     Initializes delegates which sets the an appropriate properties according to read parameters from XML. 
        /// </summary>
        /// <param name="propertyNameToSetterMap">
        ///     A map between name of the parameter in XML and delegate which sets an appropriate property on object.
        /// </param>
        protected virtual void InitializePropertySetters(Dictionary<string, Action<string>> propertyNameToSetterMap)
        {
            propertyNameToSetterMap["size"]             = value => this.Size = uint.Parse(value);
            propertyNameToSetterMap["duration"]         = value => this.Duration = ParsingHelper.ParseTimeSpan(value);
            propertyNameToSetterMap["bitrate"]          = value => this.Bitrate = uint.Parse(value);
            propertyNameToSetterMap["sampleFrequency"]  = value => this.SampleFrequency = uint.Parse(value);
            propertyNameToSetterMap["bitsPerSample"]    = value => this.BitsPerSample = uint.Parse(value);
            propertyNameToSetterMap["nrAudioChannels"]  = value => this.NumberOfAudioChannels = uint.Parse(value);
            propertyNameToSetterMap["resolution"]       = value => this.Resolution = ParsingHelper.ParseResolution(value);
            propertyNameToSetterMap["colorDepth"]       = value => this.ColorDepth = uint.Parse(value);
            propertyNameToSetterMap["protocolInfo"]     = value => this.ProtocolInfo = value;
            propertyNameToSetterMap["protection"]       = value => this.Protection = value;
            propertyNameToSetterMap["importUri"]        = value => this.ImportUri = value;
        }

        private void EnsurePropertySettersInititalized()
        {
            if (this.propertySetters == null)
            {
                this.propertySetters = new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase);

                this.InitializePropertySetters(this.propertySetters);
            }
        }

        #endregion
    }
}
