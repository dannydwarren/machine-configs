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
        private readonly IAppConfigurator appConfigurator;
        private readonly IConsoleLogger consoleLogger;

        public MachineConfigurator(ISystemInitializer systemInitializer,
            IManifestRepository manifestRepository,
            IAppInstaller appInstaller,
            IDownloadAppInstaller downloadAppInstaller,
            IAppConfigurator appConfigurator,
            IConsoleLogger consoleLogger)
        {
            this.systemInitializer = systemInitializer;
            this.manifestRepository = manifestRepository;
            this.appInstaller = appInstaller;
            this.downloadAppInstaller = downloadAppInstaller;
            this.appConfigurator = appConfigurator;
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

            consoleLogger.Debug($"Loaded {manifest.Apps.Count} {nameof(manifest.Apps)}");

            await InstallAppsAsync(manifest.Apps);
        }

        private async Task InstallAppsAsync<TApp>(List<TApp> apps)
            where TApp : IApp
        {
            foreach (var app in apps)
            {
                if (app is IDownloadApp downloadApp)
                {
                    await downloadAppInstaller.InstallAsync(downloadApp);
                }
                else
                {
                    await appInstaller.InstallOrUpgradeAsync(app);
                }

                appConfigurator.Configure(app);
            }
        }
    }
}
