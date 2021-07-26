using System;
using System.Collections.Generic;
using Configurator.Git;
using Configurator.PowerShell;
using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.Installers;
using Configurator.Utilities;

namespace Configurator
{
    public interface IMachineConfigurator
    {
        Task ExecuteAsync();
    }

    public class MachineConfigurator : IMachineConfigurator
    {
        private readonly IPowerShellConfiguration powerShellConfiguration;
        private readonly IManifestRepository manifestRepository;
        private readonly IGitConfiguration gitConfiguration;
        private readonly IGitconfigRepository gitconfigRepository;
        private readonly IAppInstaller appInstaller;
        private readonly IDownloadInstaller downloadInstaller;
        private readonly IConsoleLogger consoleLogger;

        public MachineConfigurator(IPowerShellConfiguration powerShellConfiguration,
            IManifestRepository manifestRepository,
            IGitConfiguration gitConfiguration,
            IGitconfigRepository gitconfigRepository,
            IAppInstaller appInstaller,
            IDownloadInstaller downloadInstaller,
            IConsoleLogger consoleLogger)
        {
            this.powerShellConfiguration = powerShellConfiguration;
            this.manifestRepository = manifestRepository;
            this.gitConfiguration = gitConfiguration;
            this.gitconfigRepository = gitconfigRepository;
            this.appInstaller = appInstaller;
            this.downloadInstaller = downloadInstaller;
            this.consoleLogger = consoleLogger;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                await ExecuteInternalAsync();
            }
            catch (Exception e)
            {
                consoleLogger.Error(e.ToString());
            }
        }

        private async Task ExecuteInternalAsync()
        {
            await powerShellConfiguration.SetExecutionPolicyAsync();

            var manifest = await manifestRepository.LoadAsync();

            await InstallPowerShellAppPackages(manifest.PowerShellAppPackages);
            await IncludeCustomGitconfigsAsync();
            await InstallWingetAppsAsync(manifest.WingetApps);
            await InstallScoopAppsAsync(manifest.ScoopApps);
        }

        private async Task InstallPowerShellAppPackages(List<PowerShellAppPackage> powerShellAppPackages)
        {
            foreach (var appPackage in powerShellAppPackages)
            {
                await downloadInstaller.InstallAsync(appPackage);
            }
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
