using System.IO;

namespace Emmersion.Http
{
    public interface IHttpStreamResponse
    {
        int StatusCode { get; }
        HttpHeaders Headers { get; }
        Stream Stream { get; }
    }

    public class HttpStreamResponse : IHttpStreamResponse
    {
        public HttpStreamResponse(int statusCode, HttpHeaders headers, Stream stream)
        {
            StatusCode = statusCode;
            Headers = headers;
            Stream = stream;
        }

        public int StatusCode { get; }
        public HttpHeaders Headers { get; }
        public Stream Stream { get; }
    }
}
