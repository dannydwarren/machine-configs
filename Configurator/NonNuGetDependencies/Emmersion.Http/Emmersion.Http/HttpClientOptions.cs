namespace Emmersion.Http
{
    public class HttpClientOptions
    {
        public HttpClientOptions()
        {
            DefaultTimeoutMilliseconds = 0;
            AllowAutoRedirect = true;
        }

        public int DefaultTimeoutMilliseconds { get; set; }
        public bool AllowAutoRedirect { get; set; }
    }
}