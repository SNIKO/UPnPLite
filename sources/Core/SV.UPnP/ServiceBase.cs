
namespace SV.UPnP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class ServiceBase
    {
        #region Fields

        private readonly ServiceInfo serviceInfo;

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
            var request = WebRequest.Create(this.serviceInfo.BaseURL + this.serviceInfo.ControlURL);
            request.Method = "POST";
            request.ContentType = "text/xml; charset=\"utf-8\"";
            request.Headers["SOAPACTION"] = "\"{0}#{1}\"".F(this.serviceInfo.ServiceType, action);

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
            var data = Encoding.UTF8.GetBytes(stEnvelope);

            using (var stream = await request.GetRequestStreamAsync())
            {                
                stream.Write(data, 0, data.Length);
            }

            try
            {
                var response = await request.GetResponseAsync();                
                var result = new Dictionary<string, string>();
                var argumentElements = XDocument.Load(response.GetResponseStream()).Descendants(u + "argumentName");

                foreach (var argumentElement in argumentElements)
                {
                    result[argumentElement.Name.LocalName.ToUpper()] = argumentElement.Value;
                }

                return result;
            }
            catch (WebException ex)
            {
                var c = XNamespace.Get("urn:schemas-upnp-org:control-1-0");
                var doc = XDocument.Load(ex.Response.GetResponseStream());
                var errorCodeElement = doc.Descendants(c + "errorCode").FirstOrDefault();
                var errorDesctiptionElement = doc.Descendants(c + "errorDescription").FirstOrDefault();

                throw new DeviceException((DeviceError)Convert.ToInt32(errorCodeElement.Value), errorDesctiptionElement.Value, ex);
            }
        }

        #endregion
    }
}
