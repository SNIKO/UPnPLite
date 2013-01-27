
namespace SV.UPnP
{
    using System;
    using Windows.Foundation;

    public class DeviceIcon
    {
        #region Properties

        public string Type { get; internal set; }

        public string ColorDepth { get; internal set; }

        public Size Size { get; internal set; }

        public Uri Url { get; internal set; }

        #endregion
    }
}
