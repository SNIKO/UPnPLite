
namespace SV.UPnPLite.Extensions
{
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    ///     Defines extension methods for <see cref="WebRequest"/>.
    /// </summary>
    public static class WebRequestExtensions
    {
        public static Task<Stream> GetRequestStreamAsync(this WebRequest instance)
        {
            var task = Task.Factory.FromAsync(instance.BeginGetRequestStream, asyncResult => instance.EndGetRequestStream(asyncResult), null);

            return task;
        }

        public static Task<WebResponse> GetResponseAsync(this WebRequest instance)
        {
            var task = Task.Factory.FromAsync(instance.BeginGetResponse, asyncResult => instance.EndGetResponse(asyncResult), null);

            return task;
        }
    }
}
