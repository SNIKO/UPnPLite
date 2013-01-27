
namespace SV.UPnPLite.Protocols.SSDP
{
    using SV.UPnPLite.Protocols.SSDP.Messages;
    using System;

    /// <summary>
    ///     A Desktop implementation of the SSDP protocol.
    /// </summary>
    internal class SSDPServer : ISSDPServer
    {
        #region Fields

        private static readonly object instanceSyncObject = new object();

        private static ISSDPServer instance;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SSDPServer" /> class.
        /// </summary>
        private SSDPServer()
        {
        }

        #endregion

        /// <summary>
        ///     Gets a singletone instance of the <see cref="SSDPServer"/>.
        /// </summary>
        public static ISSDPServer Instance
        {
            get
            {
                lock (instanceSyncObject)
                {
                    if (instance == null)
                    {
                        instance = new SSDPServer();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        ///     An observable collection which contains notifications from devices.
        /// </summary>
        public IObservable<NotifyMessage> NotifyMessages { get; private set; }

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
        public IObservable<SearchResponseMessage> Search(string searchTarget, int timeForResponse)
        {
            throw new NotImplementedException();
        }
    }
}
