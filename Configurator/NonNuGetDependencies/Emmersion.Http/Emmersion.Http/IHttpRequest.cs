using System.Net.Http;

namespace Emmersion.Http
{
    public interface IHttpRequest
    {
        HttpMethod Method { get; set; }
        string Url { get; set; }
        HttpHeaders Headers { get; set; }

        bool HasContent();
        HttpContent GetContent();
    }
}
