using Configurator.Installers;
using Configurator.Lists;
using Configurator.PowerShell;
using Configurator.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Configurator
{
    public interface IDannyConfig
    {
        Task ExecuteAsync();
    }

    public class MachineConfigurator : IDannyConfig
    {
        private readonly IArguments arguments;
        private readonly IPowerShellConfiguration powerShellConfiguration;
        private readonly IScoopInstaller scoopInstaller;
        private readonly IScoopList scoopList;

        public MachineConfigurator(IArguments arguments,
            IPowerShellConfiguration powerShellConfiguration,
            IScoopInstaller scoopInstaller,
            IScoopList scoopList)
        {
            this.arguments = arguments;
            this.powerShellConfiguration = powerShellConfiguration;
            this.scoopInstaller = scoopInstaller;
            this.scoopList = scoopList;
        }

        public async Task ExecuteAsync()
        {
            await powerShellConfiguration.SetExecutionPolicyAsync();

            var scoopAppsToInstall = (await scoopList.LoadAsync())
                .Where(x => x.Environment.HasFlag(arguments.Environment))
                .ToList();

            foreach (var scoopApp in scoopAppsToInstall)
            {
                await scoopInstaller.InstallAsync(scoopApp.AppId);
            }
        }
    }
}
