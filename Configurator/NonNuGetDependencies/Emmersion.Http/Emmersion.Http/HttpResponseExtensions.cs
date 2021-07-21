namespace Emmersion.Http
{
    public static class HttpResponseExtensions
    {
        public static T DeserializeJsonBody<T>(this HttpResponse response)
        {
            return CamelCaseJsonSerializer.Deserialize<T>(response.Body);
        }
    }
}