using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Configurator
{
    public class Cli
    {
        private readonly IDependencyBootstrapper dependencyBootstrapper;

        public Cli(IDependencyBootstrapper dependencyBootstrapper)
        {
            this.dependencyBootstrapper = dependencyBootstrapper;
        }

        public async Task RunConfiguratorAsync(string? manifestPath = null, List<string>? environments = null, string? downloadsDir = null)
        {
            var services = await dependencyBootstrapper.InitializeAsync(manifestPath, environments, downloadsDir);
            var configurator = services.GetRequiredService<IMachineConfigurator>();

            await configurator.ExecuteAsync();
        }
    }
}
