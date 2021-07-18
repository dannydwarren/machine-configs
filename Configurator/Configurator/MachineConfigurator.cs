using Configurator.PowerShell;
using Configurator.Scoop;
using System.Threading.Tasks;

namespace Configurator
{
    public interface IMachineConfigurator
    {
        Task ExecuteAsync();
    }

    public class MachineConfigurator : IMachineConfigurator
    {
        private readonly IPowerShellConfiguration powerShellConfiguration;
        private readonly IScoopInstaller scoopInstaller;
        private readonly IScoopAppRepository scoopList;

        public MachineConfigurator(IPowerShellConfiguration powerShellConfiguration,
            IScoopInstaller scoopInstaller,
            IScoopAppRepository scoopList)
        {
            this.powerShellConfiguration = powerShellConfiguration;
            this.scoopInstaller = scoopInstaller;
            this.scoopList = scoopList;
        }

        public async Task ExecuteAsync()
        {
            await powerShellConfiguration.SetExecutionPolicyAsync();

            await InstallScoopAppsAsync();
        }

        private async Task InstallScoopAppsAsync()
        {
            var scoopAppsToInstall = await scoopList.LoadAsync();

            foreach (var scoopApp in scoopAppsToInstall)
            {
                await scoopInstaller.InstallAsync(scoopApp.AppId);
            }
        }
    }
}
