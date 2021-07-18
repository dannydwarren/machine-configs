using Configurator.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Configurator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = ConfigureServices();

            var config = services.GetRequiredService<IDannyConfig>();

            await config.ExecuteAsync();
        }


        private static ServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();
            DependencyInjectionConfig.ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
