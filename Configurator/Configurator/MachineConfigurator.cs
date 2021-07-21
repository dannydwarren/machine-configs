using Configurator.Git;
using Configurator.PowerShell;
using Configurator.Scoop;
using System.Threading.Tasks;
using Configurator.Winget;

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
        private readonly IScoopAppRepository scoopAppRepository;
        private readonly IGitConfiguration gitConfiguration;
        private readonly IGitconfigRepository gitconfigRepository;
        private readonly IWingetAppRepository wingetAppRepository;
        private readonly IWingetAppInstaller wingetAppInstaller;

        public MachineConfigurator(IPowerShellConfiguration powerShellConfiguration,
            IScoopInstaller scoopInstaller,
            IScoopAppRepository scoopAppRepository,
            IGitConfiguration gitConfiguration,
            IGitconfigRepository gitconfigRepository,
            IWingetAppRepository wingetAppRepository,
            IWingetAppInstaller wingetAppInstaller)
        {
            this.powerShellConfiguration = powerShellConfiguration;
            this.scoopInstaller = scoopInstaller;
            this.scoopAppRepository = scoopAppRepository;
            this.gitConfiguration = gitConfiguration;
            this.gitconfigRepository = gitconfigRepository;
            this.wingetAppRepository = wingetAppRepository;
            this.wingetAppInstaller = wingetAppInstaller;
        }

        public async Task ExecuteAsync()
        {
            await powerShellConfiguration.SetExecutionPolicyAsync();

            await IncludeCustomGitconfigsAsync();
            await InstallWingetAppsAsync();
            await InstallScoopAppsAsync();
        }

        private async Task IncludeCustomGitconfigsAsync()
        {
            var gitconfigs = await gitconfigRepository.LoadAsync();

            foreach (var gitconfig in gitconfigs)
            {
                await gitConfiguration.IncludeGitconfigAsync(gitconfig.Path);
            }
        }

        private async Task InstallWingetAppsAsync()
        {
            var wingetApps = await wingetAppRepository.LoadAsync();

            foreach (var wingetApp in wingetApps)
            {
                await wingetAppInstaller.InstallAsync(wingetApp);
            }
        }

        private async Task InstallScoopAppsAsync()
        {
            var scoopAppsToInstall = await scoopAppRepository.LoadAsync();

            foreach (var scoopApp in scoopAppsToInstall)
            {
                await scoopInstaller.InstallAsync(scoopApp.AppId);
            }
        }
    }
}
