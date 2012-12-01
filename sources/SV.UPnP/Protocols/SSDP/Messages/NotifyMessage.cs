

namespace SV.UPnP.Protocols.SSDP.Messages
{
    /// <summary>
    ///     Defines the NOTIFY ssdp message.
    /// </summary>
    internal class NotifyMessage : SSDPMessage
    {
        public string NotificationType { get; set; }

        public NotifyMessageType NotificationSubtype { get; set; }

        public int NextBootId { get; set; }
    }
}
