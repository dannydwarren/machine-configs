using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.Downloaders;
using Configurator.PowerShell;
using Configurator.Utilities;

namespace Configurator.Installers
{
    public interface IDownloadAppInstaller
    {
        Task InstallAsync(IDownloadApp app);
    }

    public class DownloadAppInstaller : IDownloadAppInstaller
    {
        private readonly IConsoleLogger consoleLogger;
        private readonly IPowerShell powerShell;
        private readonly IDownloaderFactory downloaderFactory;

        public DownloadAppInstaller(IConsoleLogger consoleLogger,
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

            app.DownloadedFilePath = await downloader.DownloadAsync(app.DownloaderArgs.ToString()!);

            if (app.VerificationScript == null)
            {
                await powerShell.ExecuteAsync(app.InstallScript);
            }
            else
            {
                await powerShell.ExecuteAsync(app.InstallScript, app.VerificationScript);
            }

            consoleLogger.Result($"Installed '{app.AppId}'");
        }
    }
}
