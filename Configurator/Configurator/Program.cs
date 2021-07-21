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
                environment: InstallEnvironment.Personal,
                scoopAppsPath: @"C:\src\machine-configs\install\ScoopApps.csv",
                gitconfigsPath: @"C:\src\machine-configs\git\Gitconfigs.csv",
                downloadsDir: @"C:\Users\danny\Downloads"));

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
