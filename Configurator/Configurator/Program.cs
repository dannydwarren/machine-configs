using System.Collections.Generic;
using Configurator.Configuration;
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
                // manifestPath: @"https://raw.githubusercontent.com/dannydwarren/machine-configs/main/manifests/danny.manifest.json",
                manifestPath: @"https://raw.githubusercontent.com/dannydwarren/machine-configs/main/manifests/manifest_test.json",
                environments: new List<string> {"Personal"}
            ));

            var config = services.GetRequiredService<IMachineConfigurator>();

            await config.ExecuteAsync();

            await services.DisposeAsync();
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
