

namespace SV.UPnPLite.Protocols.SSDP.Messages
{
    using SV.UPnPLite.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Defines the NOTIFY ssdp message.
    /// </summary>
    internal class NotifyMessage : SSDPMessage
    {
        public string Host { get; set; }

        /// <summary>
        ///     Gets the type of source which sent a message.
        /// </summary>
        public string NotificationType { get; private set; }

        /// <summary>
        ///     Gets the type of the notification.
        /// </summary>
        public NotifyMessageType NotificationSubtype { get; private set; }

        /// <summary>
        ///     Gets value that the device intends to use in the subsequent device and service announcement messages.
        /// </summary>
        public int NextBootId { get; private set; }

        /// <summary>
        ///     Creates a new instance of <see cref="NotifyMessage"/> from notify message.
        /// </summary>
        /// <param name="message">
        ///     The notify message received from a device.
        /// </param>
        /// <returns>
        ///     A new instance of <see cref="NotifyMessage"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="message"/> is not valid notify message.
        /// </exception>
        internal static NotifyMessage Create(string message)
        {
            var notifyMessage = new NotifyMessage();

            var lines = message.SplitIntoLines();
            if (lines.Count() > 1)
            {
                var statusString = lines[0];
                var headers = ParseHeaders(lines.Skip(1));

                if (statusString.StartsWith("notify", StringComparison.OrdinalIgnoreCase))
                {
					try
					{
						notifyMessage.Host 				  = headers.GetValue<string>("HOST");
						notifyMessage.NotificationType 	  = headers.GetValue<string>("NT");
						notifyMessage.NotificationSubtype = ParseNotifyType(headers.GetValue<string>("NTS"));
						notifyMessage.USN 				  = headers.GetValue<string>("USN");

						notifyMessage.Location 	 = headers.GetValueOrDefault<string>("LOCATION");
						notifyMessage.MaxAge 	 = ParseMaxAge(headers.GetValueOrDefault<string>("CACHE-CONTROL"));
						notifyMessage.Server 	 = headers.GetValueOrDefault<string>("SERVER");
						notifyMessage.BootId 	 = headers.GetValueOrDefault<int>("BOOTID.UPNP.ORG");
						notifyMessage.NextBootId = headers.GetValueOrDefault<int>("NEXTBOOTID.UPNP.ORG");
						notifyMessage.ConfigId 	 = headers.GetValueOrDefault<int>("CONFIGID.UPNP.ORG");
						notifyMessage.SearchPort = headers.GetValueOrDefault<int>("SEARCHPORT.UPNP.ORG");
					}
					catch (KeyNotFoundException ex)
					{
						throw new ArgumentException("The given message is not valid notify message", "message", ex);
					}
					catch (FormatException ex)
					{
						throw new ArgumentException("The given message is not valid notify message", "message", ex);
					}
                }
            }

            return notifyMessage;
        }

        private static NotifyMessageType ParseNotifyType(string notifyType)
        {
            NotifyMessageType result;

            switch (notifyType.ToUpper())
            {
                case "SSDP:ALIVE":
                    result = NotifyMessageType.Alive;
                    break;
                case "SSDP:BYEBYE":
                    result = NotifyMessageType.ByeBye;
                    break;
                case "SSDP:UPDATE":
                    result = NotifyMessageType.Update;
                    break;
                default:
                    throw new FormatException("Unknown notification type: '{0}'".F(notifyType));
            }

            return result;
        }
    }
}
