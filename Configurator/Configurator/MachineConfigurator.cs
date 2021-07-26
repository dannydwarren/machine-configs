using System;
using System.Collections.Generic;
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
        private readonly IAppInstaller appInstaller;
        private readonly IDownloadInstaller downloadInstaller;
        private readonly IConsoleLogger consoleLogger;

        public MachineConfigurator(IPowerShellConfiguration powerShellConfiguration,
            IManifestRepository manifestRepository,
            IAppInstaller appInstaller,
            IDownloadInstaller downloadInstaller,
            IConsoleLogger consoleLogger)
        {
            this.powerShellConfiguration = powerShellConfiguration;
            this.manifestRepository = manifestRepository;
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

            await InstallDownloadApps(manifest.PowerShellAppPackages);
            await InstallAppsAsync(manifest.WingetApps);
            await InstallAppsAsync(manifest.ScoopApps);
            await InstallAppsAsync(manifest.Gitconfigs);
        }

        private async Task InstallDownloadApps<TDownloadApp>(List<TDownloadApp> downloadApps)
            where TDownloadApp : IDownloadApp
        {
            foreach (var downloadApp in downloadApps)
            {
                await downloadInstaller.InstallAsync(downloadApp);
            }
        }

        private async Task InstallAppsAsync<TApp>(List<TApp> apps)
            where TApp : IApp
        {
            foreach (var app in apps)
            {
                await appInstaller.InstallAsync(app);
            }
        }
    }
}
