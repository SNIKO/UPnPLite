
namespace SV.UPnP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    ///     The base class for all UPnP device's services.
    /// </summary>
    public class ServiceBase
    {
        #region Fields

        private readonly ServiceInfo serviceInfo;

        private readonly Uri controlUri;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceBase" /> class.
        /// </summary>
        /// <param name="serviceInfo">
        ///     The service info which defines parameters of the service to manage.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///      <paramref name="serviceInfo"/> is <c>null</c>.
        /// </exception>
        public ServiceBase(ServiceInfo serviceInfo)
        {
            serviceInfo.EnsureNotNull("serviceInfo");

            this.serviceInfo = serviceInfo;
            this.controlUri = new Uri(new Uri(this.serviceInfo.BaseURL), this.serviceInfo.ControlURL);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Invokes an <paramref name="action"/> at the device's service.
        /// </summary>
        /// <param name="action">
        ///     An action to invoke.
        /// </param>
        /// <param name="parameters">
        ///     Invocation parameters.
        /// </param>
        /// <returns>
        ///     A dictionary with result of the action.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     An <see cref="action"/> is <c>null</c> or empty.
        /// </exception>
        protected async Task<Dictionary<string, string>> InvokeActionAsync(string action, Dictionary<string, object> parameters)
        {
            var requestXml = this.CreateActionRequest(action, parameters);
            var data = Encoding.UTF8.GetBytes(requestXml);

            var request = WebRequest.Create(this.controlUri);
            request.Method = "POST";
            request.ContentType = "text/xml; charset=\"utf-8\"";
            request.Headers["SOAPACTION"] = "\"{0}#{1}\"".F(this.serviceInfo.ServiceType, action);
            using (var stream = await request.GetRequestStreamAsync())
            {
                stream.Write(data, 0, data.Length);
            }

            try
            {
                var response = await request.GetResponseAsync();
                var result = ParseActionResponse(action, XDocument.Load(response.GetResponseStream()));

                return result;
            }
            catch (WebException ex)
            {
                var c = XNamespace.Get("urn:schemas-upnp-org:control-1-0");
                var doc = XDocument.Load(ex.Response.GetResponseStream());
                var errorCodeElement = doc.Descendants(c + "errorCode").FirstOrDefault();
                var errorDesctiptionElement = doc.Descendants(c + "errorDescription").FirstOrDefault();

                throw new DeviceException(Convert.ToInt32(errorCodeElement.Value), errorDesctiptionElement.Value, ex);
            }
        }

        private string CreateActionRequest(string action, Dictionary<string, object> parameters)
        {
            var s = XNamespace.Get("http://schemas.xmlsoap.org/soap/envelope/");
            var u = XNamespace.Get(this.serviceInfo.ServiceType);
            var encodingStyle = XNamespace.Get("http://schemas.xmlsoap.org/soap/encoding/");

            var actionElement = new XElement(u + action, new XAttribute(XNamespace.Xmlns + "u", u.NamespaceName));

            foreach (var parameter in parameters)
            {
                actionElement.Add(new XElement(parameter.Key, parameter.Value));
            }

            var envelope = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement(s + "Envelope",
                    new XAttribute(XNamespace.Xmlns + "s", s.NamespaceName),
                    new XAttribute(s + "encodingStyle", encodingStyle.NamespaceName),
                    new XElement(s + "Body", actionElement)));

            var stEnvelope = envelope.ToStringWithDeclaration();
            return stEnvelope;
        }

        private Dictionary<string, string> ParseActionResponse(string action, XDocument response)
        {
            var u = XNamespace.Get(this.serviceInfo.ServiceType);
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var responseNode = response.Descendants(u + "{0}Response".F(action)).First();

            foreach (var argumentElement in responseNode.Descendants())
            {
                result[argumentElement.Name.LocalName] = argumentElement.Value;
            }

            return result;
        }

        #endregion
    }
}
