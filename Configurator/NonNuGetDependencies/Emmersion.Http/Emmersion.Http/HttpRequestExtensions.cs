using System;
using System.Text;

namespace Emmersion.Http
{
    public static class HttpRequestExtensions
    {
        public static void AddJsonBody(this HttpRequest request, object bodyObject)
        {
            request.Body = CamelCaseJsonSerializer.Serialize(bodyObject);
            request.Headers.Add("Content-Type", "application/json");
        }

        public static void AddBasicAuthentication(this IHttpRequest request, string username, string password)
        {
            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            request.Headers.Add("Authorization", $"Basic {encodedCredentials}");
        }
    }
}
