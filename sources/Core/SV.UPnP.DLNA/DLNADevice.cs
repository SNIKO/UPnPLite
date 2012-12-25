
namespace SV.UPnP.DLNA
{
    using System;
    using System.Collections.Generic;
    using Windows.Foundation;

    /// <summary>
    ///     The base class for all DLNA devices.
    /// </summary>
    public abstract class DLNADevice
    {
        #region Properties

        public string Name { get; internal set; }

        public string Manufacturer { get; internal set; }

        public UPnPVersion Version { get; internal set; }

        public string UDN { get; internal set; }

        public Uri Icon { get; internal set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DLNADevice" /> class.
        /// </summary>
        /// <param name="deviceInfo">
        ///     Defines parameters of the device.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="deviceInfo"/> is <c>null</c>.
        /// </exception>
        protected DLNADevice(DeviceInfo deviceInfo)
        {
            deviceInfo.EnsureNotNull("deviceInfo");

            this.Name = deviceInfo.FriendlyName;
            this.Manufacturer = deviceInfo.Manufacturer;
            this.UDN = deviceInfo.UDN;
            this.Version = deviceInfo.DeviceVersion;
            this.Icon = GetLargestIcon(deviceInfo.Icons);
        }

        private static Uri GetLargestIcon(IEnumerable<DeviceIcon> icons)
        {
            Uri result = null;
            var maxSize = new Size(0, 0);

            if (icons != null)
            {
                foreach (var icon in icons)
                {
                    if (icon.Size.Width > maxSize.Width)
                    {
                        maxSize = icon.Size;
                        result = icon.Url;
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
