using System.Net.Http;
using System.Text;

namespace Emmersion.Http
{
    public class HttpRequest : IHttpRequest
    {
        public HttpRequest()
        {
            Headers = new HttpHeaders();
        }

        public HttpMethod Method { get; set; }
        public string Url { get; set; }
        public HttpHeaders Headers { get; set; }
        public string Body { get; set; }

        public bool HasContent() => Body != null;

        public HttpContent GetContent() => new ByteArrayContent(Encoding.UTF8.GetBytes(Body));
    }
}
