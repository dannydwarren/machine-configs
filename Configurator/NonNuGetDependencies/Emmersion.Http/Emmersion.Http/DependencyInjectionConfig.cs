using Microsoft.Extensions.DependencyInjection;

namespace Emmersion.Http
{
    public class DependencyInjectionConfig
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpClient, HttpClient>();
        }
    }
}