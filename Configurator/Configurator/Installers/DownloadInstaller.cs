using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.Downloaders;
using Configurator.PowerShell;
using Configurator.Utilities;

namespace Configurator.Installers
{
    public interface IDownloadInstaller
    {
        Task InstallAsync(IDownloadApp app);
    }

    public class DownloadInstaller : IDownloadInstaller
    {
        private readonly IConsoleLogger consoleLogger;
        private readonly IPowerShell powerShell;
        private readonly IDownloaderFactory downloaderFactory;

        public DownloadInstaller(IConsoleLogger consoleLogger,
            IPowerShell powerShell,
            IDownloaderFactory downloaderFactory)
        {
            this.consoleLogger = consoleLogger;
            this.powerShell = powerShell;
            this.downloaderFactory = downloaderFactory;
        }

        public async Task InstallAsync(IDownloadApp app)
        {
            consoleLogger.Info($"Installing '{app.AppId}'");

            IDownloader downloader = downloaderFactory.GetDownloader(app.Downloader);

            var downloadedFilePath = await downloader.DownloadAsync(app.DownloaderArgs.ToString()!);

            var installScript = $"{app.InstallScript} {downloadedFilePath}";

            if (app.VerificationScript == null)
            {
                await powerShell.ExecuteAsync(installScript);
            }
            else
            {
                await powerShell.ExecuteAsync(installScript, app.VerificationScript);
            }

            consoleLogger.Result($"Installed '{app.AppId}'");
        }
    }
}
