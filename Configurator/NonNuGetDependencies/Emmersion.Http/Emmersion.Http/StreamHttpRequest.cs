using System.IO;
using System.Net.Http;

namespace Emmersion.Http
{
    public class StreamHttpRequest: IHttpRequest
    {
        public StreamHttpRequest()
        {
            Headers = new HttpHeaders();
        }

        public HttpMethod Method { get; set; }
        public string Url { get; set; }
        public HttpHeaders Headers { get; set; }
        public Stream Body { get; set; }

        public bool HasContent() => Body != null;

        public HttpContent GetContent() => new StreamContent(Body);
    }
}
