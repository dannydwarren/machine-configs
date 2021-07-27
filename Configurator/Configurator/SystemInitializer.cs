using System.Threading.Tasks;
using Configurator.Installers;
using Configurator.PowerShell;

namespace Configurator
{
    public interface ISystemInitializer
    {
        Task InitializeAsync();
    }

    public class SystemInitializer : ISystemInitializer
    {
        private readonly IPowerShellConfiguration powerShellConfiguration;
        private readonly IWingetCliInstaller wingetCliInstaller;

        public SystemInitializer(IPowerShellConfiguration powerShellConfiguration,
            IWingetCliInstaller wingetCliInstaller)
        {
            this.powerShellConfiguration = powerShellConfiguration;
            this.wingetCliInstaller = wingetCliInstaller;
        }

        public async Task InitializeAsync()
        {
            await powerShellConfiguration.SetExecutionPolicyAsync();
            await wingetCliInstaller.InstallAsync();
        }
    }
}
