using Configurator.Configuration;
using Configurator.Scoop;
using Configurator.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Configurator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = ConfigureServices(new Arguments(
                scoopAppsPath: @"C:\src\machine-configs\install\ScoopApps.csv",
                environment: InstallEnvironment.Personal
            ));

            var config = services.GetRequiredService<IDannyConfig>();

            await config.ExecuteAsync();
        }

        private static ServiceProvider ConfigureServices(Arguments arguments)
        {
            var serviceCollection = new ServiceCollection();
            DependencyInjectionConfig.ConfigureServices(serviceCollection);
            serviceCollection.AddSingleton<IArguments>(arguments);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
