
namespace SV.UPnP
{
    using System.Collections.Generic;

    public class DeviceInfo
    {
        public string BaseURL { get; internal set; }

        public string DeviceType { get; internal set; }

        public UPnPVersion DeviceVersion { get; internal set; }

        public string FriendlyName { get; internal set; }

        public string Manufacturer { get; internal set; }

        public string UDN { get; internal set; }

        public IEnumerable<DeviceIcon> Icons { get; internal set; }
        
        public IEnumerable<ServiceInfo> Services { get; internal set; }
    }
}
