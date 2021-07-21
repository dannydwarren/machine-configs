using System.Text.Json;

namespace Emmersion.Http
{
    internal class CamelCaseJsonSerializer
    {
        public static string Serialize(object input)
        {
            return JsonSerializer.Serialize(input, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        public static T Deserialize<T>(string input)
        {
            return JsonSerializer.Deserialize<T>(input, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}