namespace Emmersion.Http
{
    public class HttpResponse
    {
        public HttpResponse(int statusCode, HttpHeaders headers, string body)
        {
            StatusCode = statusCode;
            Headers = headers;
            Body = body;
        }

        public int StatusCode { get; }
        public HttpHeaders Headers { get; }
        public string Body { get; }
    }
}