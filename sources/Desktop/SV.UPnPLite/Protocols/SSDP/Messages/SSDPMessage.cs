
namespace SV.UPnPLite.Protocols.SSDP.Messages
{
    using SV.UPnPLite.Extensions;
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The base class for all SSDP messages.
    /// </summary>
    internal class SSDPMessage
    {
        public int MaxAge { get; set; }

        public string Location { get; set; }

        public string Server { get; set; }

        public int SearchPort { get; set; }

        public string USN { get; set; }

        public int BootId { get; set; }

        public int ConfigId { get; set; }

        /// <summary>
        ///     Retrieves the MaxAge property from the CACHE-CONTROL header.
        /// </summary>
        /// <param name="cacheControlSettings">
        ///     The value of the SSDP message's CACHE-CONTROL header.      
        /// </param>
        /// <returns>
        ///     The retrieved MaxAge property.
        /// </returns>
        /// <exception cref="FormatException">
        ///     The cache control settings are in bad format.
        /// </exception>
        protected static int ParseMaxAge(string cacheControlSettings)
        {
            if (string.IsNullOrEmpty(cacheControlSettings))
            {
                return 0;
            }

            var keyValue = cacheControlSettings.Split('=');
            if (keyValue.Length == 2)
            {
                return Convert.ToInt32(keyValue[1]);
            }
            else
            {
                throw new FormatException("The cache control settings are in bad format");
            }
        }

        /// <summary>
        ///     Parses the HTTP headers.
        /// </summary>
        /// <param name="headerLines">
        ///     The HTTP headers.
        /// </param>
        /// <returns>
        ///     The <see cref="IReadOnlyDictionary{TKey,TValue}"/> instance which represents parsed headers as key-value pairs.
        /// </returns>
        protected static IReadOnlyDictionary<string, string> ParseHeaders(IEnumerable<string> headerLines)
        {
            var result = new Dictionary<string, string>();

            foreach (var headerLine in headerLines)
            {
                var keyValue = headerLine.Split(new[] { ":" }, 2, StringSplitOptions.None);

                result[keyValue[0].ToUpper()] = keyValue[1].TrimStart(' ');
            }

            return result;
        }
    }
}
