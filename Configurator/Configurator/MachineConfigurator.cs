using System;
using System.Collections.Generic;
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
        private readonly ISystemInitializer systemInitializer;
        private readonly IManifestRepository manifestRepository;
        private readonly IAppInstaller appInstaller;
        private readonly IDownloadAppInstaller downloadAppInstaller;
        private readonly IConsoleLogger consoleLogger;

        public MachineConfigurator(ISystemInitializer systemInitializer,
            IManifestRepository manifestRepository,
            IAppInstaller appInstaller,
            IDownloadAppInstaller downloadAppInstaller,
            IConsoleLogger consoleLogger)
        {
            this.systemInitializer = systemInitializer;
            this.manifestRepository = manifestRepository;
            this.appInstaller = appInstaller;
            this.downloadAppInstaller = downloadAppInstaller;
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
            await systemInitializer.InitializeAsync();

            var manifest = await manifestRepository.LoadAsync();

            consoleLogger.Debug($"Loaded {manifest.PowerShellAppPackages.Count} {nameof(manifest.PowerShellAppPackages)}");
            consoleLogger.Debug($"Loaded {manifest.WingetApps.Count} {nameof(manifest.WingetApps)}");
            consoleLogger.Debug($"Loaded {manifest.ScoopBuckets.Count} {nameof(manifest.ScoopBuckets)}");
            consoleLogger.Debug($"Loaded {manifest.ScoopApps.Count} {nameof(manifest.ScoopApps)}");
            consoleLogger.Debug($"Loaded {manifest.Gitconfigs.Count} {nameof(manifest.Gitconfigs)}");

            await InstallDownloadApps(manifest.PowerShellAppPackages);
            await InstallAppsAsync(manifest.WingetApps);
            await InstallAppsAsync(manifest.ScoopBuckets);
            await InstallAppsAsync(manifest.ScoopApps);
            await InstallAppsAsync(manifest.Gitconfigs);
        }

        private async Task InstallDownloadApps<TDownloadApp>(List<TDownloadApp> downloadApps)
            where TDownloadApp : IDownloadApp
        {
            foreach (var downloadApp in downloadApps)
            {
                await downloadAppInstaller.InstallAsync(downloadApp);
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
