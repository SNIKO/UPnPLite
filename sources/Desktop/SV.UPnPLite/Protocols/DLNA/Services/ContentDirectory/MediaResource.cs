
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
	using System;
	using System.IO;
	using System.Xml;
	using SV.UPnPLite.Extensions;
	using SV.UPnPLite.Logging;
	using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory.Extensions;

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

		private ILogger logger;

		#endregion

		#region Constructors

		/// <summary>
		///		Initializes a new instance of the <see cref="MediaResource"/> class.
		/// </summary>
		/// <param name="logManager">
		///		The log manager to use for logging.
		///	</param>
		public MediaResource(ILogManager logManager = null)
		{
			if (logManager != null)
			{
				this.logger = logManager.GetLogger<MediaResource>();
			}
		}

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
		///     Deserializes the resource from a DIDL-Lite XML.
		/// </summary>
		/// <param name="resourceXml">
		///     A resource XML created according to DIDL-Lite schema.
		/// </param>
		public MediaResource Deserialize(string resourceXml)
		{
			using (var reader = XmlReader.Create(new StringReader(resourceXml), xmlReaderSettings))
			{
				reader.Read();

				while (reader.MoveToNextAttribute())
				{
					try
					{
						this.SetValue(reader.LocalName, reader.Value);
					}
					catch (FormatException ex)
					{
						this.logger.Instance().Warning(ex, "Unable to parse value '{0}' for resource key '{1}'.".F(reader.Value, reader.LocalName), "ResourceXML".As(resourceXml));
					}
					catch (OverflowException ex)
					{
						this.logger.Instance().Warning(ex, "Unable to parse value '{0}' for resource key '{1}'.".F(reader.Value, reader.LocalName), "ResourceXML".As(resourceXml));
					}
				}

				reader.MoveToElement();

				this.Uri = reader.ReadElementContentAsString();
			}

			this.Metadata = resourceXml;

			return this;
		}

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
		private void SetValue(string key, string value)
		{
			if (key.Is("size"))
			{
				this.Size = ulong.Parse(value);
			}
			else if (key.Is("duration"))
			{
				this.Duration = ParsingHelper.ParseTimeSpan(value);
			}
			else if (key.Is("bitrate"))
			{
				this.Bitrate = uint.Parse(value);
			}
			else if (key.Is("sampleFrequency"))
			{
				this.SampleFrequency = uint.Parse(value);
			}
			else if (key.Is("bitsPerSample"))
			{
				this.BitsPerSample = uint.Parse(value);
			}
			else if (key.Is("nrAudioChannels"))
			{
				this.NumberOfAudioChannels = uint.Parse(value);
			}
			else if (key.Is("resolution"))
			{
				this.Resolution = ParsingHelper.ParseResolution(value);
			}
			else if (key.Is("colorDepth"))
			{
				this.ColorDepth = uint.Parse(value);
			}
			else if (key.Is("protocolInfo"))
			{
				this.ProtocolInfo = value;
			}
			else if (key.Is("protection"))
			{
				this.Protection = value;
			}
			else if (key.Is("importUri"))
			{
				this.ImportUri = value;
			}
		}

		#endregion
	}
}
