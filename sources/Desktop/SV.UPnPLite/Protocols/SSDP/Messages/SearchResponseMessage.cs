
namespace SV.UPnPLite.Protocols.SSDP.Messages
{
    using System;

    /// <summary>
    ///     Defines a response to a MSearch request message.
    /// </summary>
    internal class SearchResponseMessage : SSDPMessage
    {
        public DateTime Date { get; set; }

        public string SearchTarget { get; set; }
    }
}
