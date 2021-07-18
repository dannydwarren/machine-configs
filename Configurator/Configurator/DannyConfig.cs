using Configurator.Installers;
using Configurator.PowerShell;
using System.Threading.Tasks;

namespace Configurator
{
    public interface IDannyConfig
    {
        Task ExecuteAsync();
    }

    public class DannyConfig : IDannyConfig
    {
        private readonly IPowerShellConfiguration powerShellConfiguration;
        private readonly IScoopInstaller scoopInstaller;

        public DannyConfig(IPowerShellConfiguration powerShellConfiguration, IScoopInstaller scoopInstaller)
        {
            this.powerShellConfiguration = powerShellConfiguration;
            this.scoopInstaller = scoopInstaller;
        }

        public async Task ExecuteAsync()
        {
            await powerShellConfiguration.SetExecutionPolicyAsync();
            await scoopInstaller.InstallAsync("mob");
        }
    }
}
