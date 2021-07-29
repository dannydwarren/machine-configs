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
        private readonly IScoopCliInstaller scoopCliInstaller;

        public SystemInitializer(IPowerShellConfiguration powerShellConfiguration,
            IWingetCliInstaller wingetCliInstaller,
            IScoopCliInstaller scoopCliInstaller)
        {
            this.powerShellConfiguration = powerShellConfiguration;
            this.wingetCliInstaller = wingetCliInstaller;
            this.scoopCliInstaller = scoopCliInstaller;
        }

        public async Task InitializeAsync()
        {
            await powerShellConfiguration.SetExecutionPolicyAsync();
            await wingetCliInstaller.InstallAsync();
            await scoopCliInstaller.InstallAsync();
        }
    }
}
