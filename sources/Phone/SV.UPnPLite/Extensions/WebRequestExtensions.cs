
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

        public static Task<WebResponse> GetResponseAsync(this WebRequest request)
        {
            var taskComplete = new TaskCompletionSource<WebResponse>();
            request.BeginGetResponse(asyncResponse =>
            {
                try
                {
                    var responseRequest = (WebRequest)asyncResponse.AsyncState;
                    var response = responseRequest.EndGetResponse(asyncResponse);

                    taskComplete.TrySetResult(response);
                }
                catch (WebException ex)
                {
                    var failedResponse = ex.Response;
                    taskComplete.TrySetResult(failedResponse);
                }
            }, request);

            return taskComplete.Task;
        }
    }
}
