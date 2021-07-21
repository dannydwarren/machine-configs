using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Emmersion.Http
{
    public interface IHttpClient
    {
        HttpResponse Execute(IHttpRequest request);
        HttpResponse Execute(IHttpRequest request, int timeoutMilliseconds);
        Task<HttpResponse> ExecuteAsync(IHttpRequest request);
        HttpStreamResponse ExecuteAsStream(IHttpRequest request);
        HttpStreamResponse ExecuteAsStream(IHttpRequest request, int timeoutMilliseconds);
        Task<HttpStreamResponse> ExecuteAsStreamAsync(IHttpRequest request);
    }

    public class HttpClient : IHttpClient, IDisposable
    {
        private readonly System.Net.Http.HttpClient client;

        public HttpClient() : this(options: null)
        {
        }

        public HttpClient(HttpClientOptions options)
        {
            if (options == null) options = new HttpClientOptions();
            var handler = new HttpClientHandler {UseCookies = false, AllowAutoRedirect = options.AllowAutoRedirect};
            client = new System.Net.Http.HttpClient(handler);
            if (options.DefaultTimeoutMilliseconds > 0)
            {
                client.Timeout = TimeSpan.FromMilliseconds(options.DefaultTimeoutMilliseconds);
            }
        }

        public void Dispose()
        {
            client?.Dispose();
        }

        public HttpResponse Execute(IHttpRequest request)
        {
            return Execute(request, timeoutMilliseconds: 0);
        }

        public HttpResponse Execute(IHttpRequest request, int timeoutMilliseconds)
        {
            var task = ExecuteAsync(request);
            if (timeoutMilliseconds > 0)
            {
                task.Wait(timeoutMilliseconds);
                if (!task.IsCompleted)
                {
                    throw new HttpTimeoutException();
                }
            }

            try
            {
                return task.Result;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Count(e => e is HttpTimeoutException) >= 1)
                {
                    throw new HttpTimeoutException();
                }

                throw;
            }
        }

        public async Task<HttpResponse> ExecuteAsync(IHttpRequest request)
        {
            var requestMessage = BuildRequestMessage(request);
            try
            {
                var response = await client.SendAsync(requestMessage).ConfigureAwait(continueOnCapturedContext: false);
                return await BuildResponse(response);
            }
            catch (TaskCanceledException)
            {
                throw new HttpTimeoutException();
            }
        }

        public HttpStreamResponse ExecuteAsStream(IHttpRequest request)
        {
            return ExecuteAsStream(request, timeoutMilliseconds: 0);
        }

        public async Task<HttpStreamResponse> ExecuteAsStreamAsync(IHttpRequest request)
        {
            var requestMessage = BuildRequestMessage(request);
            try
            {
                var response = await client.SendAsync(requestMessage).ConfigureAwait(continueOnCapturedContext: false);
                return await BuildStreamResponse(response);
            }
            catch (TaskCanceledException)
            {
                throw new HttpTimeoutException();
            }
        }

        public HttpStreamResponse ExecuteAsStream(IHttpRequest request, int timeoutMilliseconds)
        {
            var task = ExecuteAsStreamAsync(request);
            if (timeoutMilliseconds > 0)
            {
                task.Wait(timeoutMilliseconds);
                if (!task.IsCompleted)
                {
                    throw new HttpTimeoutException();
                }
            }

            try
            {
                return task.Result;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Count(e => e is HttpTimeoutException) >= 1)
                {
                    throw new HttpTimeoutException();
                }

                throw;
            }
        }

        private HttpRequestMessage BuildRequestMessage(IHttpRequest request)
        {
            var message = new HttpRequestMessage(GetRequestMethod(request.Method), request.Url);
            foreach (var headerName in request.Headers.GetAllHeaderNames().Where(IsStandardHeaderName))
            {
                message.Headers.Add(headerName, request.Headers.GetAllValues(headerName));
            }

            var acceptHeader = request.Headers.GetAllHeaderNames().FirstOrDefault(x => x.Equals("accept", StringComparison.CurrentCultureIgnoreCase));
            if (acceptHeader != null)
            {
                message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(request.Headers.GetValue(acceptHeader)));
            }

            if (request.HasContent())
            {
                message.Content = GetContent(request);
            }

            return message;
        }

        private static async Task<HttpResponse> BuildResponse(HttpResponseMessage response)
        {
            if (response == null) throw new Exception("Unable to read web response");

            return new HttpResponse((int) response.StatusCode, GetResponseHeaders(response), await response.Content.ReadAsStringAsync());
        }

        private static async Task<HttpStreamResponse> BuildStreamResponse(HttpResponseMessage response)
        {
            if (response == null) throw new Exception("Unable to read web response");

            return new HttpStreamResponse((int) response.StatusCode, GetResponseHeaders(response), await response.Content.ReadAsStreamAsync());
        }

        private static HttpContent GetContent(IHttpRequest request)
        {
            var content = request.GetContent();
            var contentTypeHeader = request.Headers.GetAllHeaderNames().FirstOrDefault(x => x.Equals("content-type", StringComparison.CurrentCultureIgnoreCase));
            if (contentTypeHeader != null)
            {
                content.Headers.ContentType = new MediaTypeHeaderValue(request.Headers.GetValue(contentTypeHeader));
            }

            return content;
        }

        private static System.Net.Http.HttpMethod GetRequestMethod(HttpMethod method)
        {
            switch (method)
            {
                case HttpMethod.GET:
                    return System.Net.Http.HttpMethod.Get;
                case HttpMethod.POST:
                    return System.Net.Http.HttpMethod.Post;
                case HttpMethod.PUT:
                    return System.Net.Http.HttpMethod.Put;
                case HttpMethod.DELETE:
                    return System.Net.Http.HttpMethod.Delete;
                default:
                    throw new ArgumentException($"Unsupported HTTP method: '{method}'");
            }
        }

        private static HttpHeaders GetResponseHeaders(HttpResponseMessage response)
        {
            var headers = new HttpHeaders();

            headers.AddAll(response.Headers);
            headers.AddAll(response.Content.Headers);
            return headers;
        }

        private static bool IsStandardHeaderName(string name)
        {
            if (name.Equals("accept", StringComparison.CurrentCultureIgnoreCase)) return false;
            if (name.Equals("content-type", StringComparison.CurrentCultureIgnoreCase)) return false;

            return true;
        }
    }
}
