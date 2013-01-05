
namespace SV.UPnP
{
    using System.Collections.Generic;

    /// <summary>
    ///     A description for a UPnP device. Contains several pieces of vendor-specific information, definitions of all embedded devices, URL for presentation 
    ///     of the device, and listings for all services, including URLs for control and eventing.
    /// </summary>
    public class DeviceInfo
    {
        /// <summary>
        ///     Gets a type of the device.
        /// </summary>
        public string DeviceType { get; internal set; }

        /// <summary>
        ///     GEts a version of the device.
        /// </summary>
        public UPnPVersion DeviceVersion { get; internal set; }

        /// <summary>
        ///     Gets a name of the device.
        /// </summary>
        public string FriendlyName { get; internal set; }

        /// <summary>
        ///     Gets a manufacturer's name of the device.
        /// </summary>
        public string Manufacturer { get; internal set; }

        /// <summary>
        ///     Gets a universally-unique identifier for the device, whether root or embedded. Must be the same over time for a specific device instance (i.e., must survive reboots).
        /// </summary>
        public string UDN { get; internal set; }

        /// <summary>
        ///     Gets the list of icons to depict device in a UI.
        /// </summary>
        public IEnumerable<DeviceIcon> Icons { get; internal set; }

        /// <summary>
        ///     Gets a list of UPnP services provided by the device.
        /// </summary>
        public IEnumerable<ServiceInfo> Services { get; internal set; }
    }
}
