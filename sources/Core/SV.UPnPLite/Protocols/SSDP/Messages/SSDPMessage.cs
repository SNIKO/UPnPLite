
namespace SV.UPnPLite.Protocols.SSDP.Messages
{
    /// <summary>
    ///     The base class for all SSDP messages.
    /// </summary>
    internal class SSDPMessage
    {
        public string Host { get; set; }

        public int MaxAge { get; set; }

        public string Location { get; set; }

        public string Server { get; set; }

        public int SearchPort { get; set; }

        public string USN { get; set; }

        public int BootId { get; set; }

        public int ConfigId { get; set; }
    }
}
