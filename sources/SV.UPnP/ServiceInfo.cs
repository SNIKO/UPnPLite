
namespace SV.UPnP
{
    public class ServiceInfo
    {
        #region Properties

        public string BaseURL { get; internal set; }

        public string ServiceType { get; internal set; }

        public string ServiceVersion { get; internal set; }

        public string DescriptionURL { get; internal set; }

        public string ControlURL { get; internal set; }

        public string EventsSunscriptionURL { get; internal set; }

        #endregion
    }
}
