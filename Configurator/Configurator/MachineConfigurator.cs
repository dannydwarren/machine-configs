using System.Collections.Generic;
using Configurator.Git;
using Configurator.PowerShell;
using System.Threading.Tasks;
using Configurator.Apps;

namespace Configurator
{
    public interface IMachineConfigurator
    {
        Task ExecuteAsync();
    }

    public class MachineConfigurator : IMachineConfigurator
    {
        private readonly IPowerShellConfiguration powerShellConfiguration;
        private readonly IAppsRepository appsRepository;
        private readonly IGitConfiguration gitConfiguration;
        private readonly IGitconfigRepository gitconfigRepository;
        private readonly IAppInstaller appInstaller;

        public MachineConfigurator(IPowerShellConfiguration powerShellConfiguration,
            IAppsRepository appsRepository,
            IGitConfiguration gitConfiguration,
            IGitconfigRepository gitconfigRepository,
            IAppInstaller appInstaller)
        {
            this.powerShellConfiguration = powerShellConfiguration;
            this.appsRepository = appsRepository;
            this.gitConfiguration = gitConfiguration;
            this.gitconfigRepository = gitconfigRepository;
            this.appInstaller = appInstaller;
        }

        public async Task ExecuteAsync()
        {
            await powerShellConfiguration.SetExecutionPolicyAsync();

            var apps = await appsRepository.LoadAsync();

            await IncludeCustomGitconfigsAsync();
            await InstallWingetAppsAsync(apps.WingetApps);
            await InstallScoopAppsAsync(apps.ScoopApps);
        }

        private async Task IncludeCustomGitconfigsAsync()
        {
            var gitconfigs = await gitconfigRepository.LoadAsync();

            foreach (var gitconfig in gitconfigs)
            {
                await gitConfiguration.IncludeGitconfigAsync(gitconfig.Path);
            }
        }

        private async Task InstallWingetAppsAsync(List<WingetApp> wingetApps)
        {
            foreach (var wingetApp in wingetApps)
            {
                await appInstaller.InstallAsync(wingetApp);
            }
        }

        private async Task InstallScoopAppsAsync(List<ScoopApp> scoopApps)
        {
            foreach (var scoopApp in scoopApps)
            {
                await appInstaller.InstallAsync(scoopApp);
            }
        }
    }
}
