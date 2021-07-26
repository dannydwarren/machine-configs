using System;
using System.Linq;
using Configurator.Configuration;
using Configurator.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Configurator.PowerShell;

namespace Configurator
{
    class Program
    {
        // manifestPath: @"https://raw.githubusercontent.com/dannydwarren/machine-configs/main/manifests/danny.manifest.json",
        static async Task Main(string manifestPath = @"https://raw.githubusercontent.com/dannydwarren/machine-configs/main/manifests/manifest_test.json", string environments = "Test")
        {
            var services = ConfigureServices(new Arguments(
                manifestPath: manifestPath,
                environments: environments.Split("|", StringSplitOptions.RemoveEmptyEntries).ToList()
            ));

            var powerShell = services.GetRequiredService<IPowerShell>();
            var result = await powerShell.ExecuteAsync("$PSVersionTable.PSVersion.ToString()");
            Console.WriteLine($"PowerShell Version: {result.AsString}");

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
