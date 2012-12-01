
namespace SV.UPnP.Protocols.SSDP
{
    using System;
    using SV.UPnP.Protocols.SSDP.Messages;

    /// <summary>
    ///     Defines members for sending/receiving SSDP messages.
    /// </summary>
    internal interface ISSDPServer
    {
        /// <summary>
        ///     An observable collection which contains notifications from devices.
        /// </summary>
        IObservable<NotifyMessage> NotifyMessages { get; }

        /// <summary>
        ///     Searches for an available devices of specified type.
        /// </summary>
        /// <param name="searchTarget">
        ///     The type of the devices to search for.
        /// </param>
        /// <param name="timeForResponse">
        ///     The time (in seconds) of a search.
        /// </param>
        /// <returns>
        ///     An observable collection which contains search results.
        /// </returns>
        IObservable<SearchResponseMessage> Search(string searchTarget, int timeForResponse);
    }
}
