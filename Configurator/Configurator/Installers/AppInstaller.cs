using System.Linq;
using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.PowerShell;
using Configurator.Utilities;

namespace Configurator.Installers
{
    public interface IAppInstaller
    {
        Task InstallOrUpgradeAsync(IApp app);
    }

    public class AppInstaller : IAppInstaller
    {
        private readonly IPowerShell powerShell;
        private readonly IConsoleLogger consoleLogger;
        private readonly IDesktopRepository desktopRepository;

        public AppInstaller(IPowerShell powerShell, IConsoleLogger consoleLogger, IDesktopRepository desktopRepository)
        {
            this.powerShell = powerShell;
            this.consoleLogger = consoleLogger;
            this.desktopRepository = desktopRepository;
        }

        public async Task InstallOrUpgradeAsync(IApp app)
        {
            consoleLogger.Info($"Installing '{app.AppId}'");
            var preInstallDesktopSystemEntries = desktopRepository.LoadSystemEntries();

            var preInstallVerificationResult = await VerifyAppAsync(app);

            if (!preInstallVerificationResult)
            {
                await powerShell.ExecuteAsync(app.InstallScript);
            }
            else if (app.UpgradeScript != null)
            {
                await powerShell.ExecuteAsync(app.UpgradeScript);
            }

            await VerifyAppAsync(app);

            var postInstallDesktopSystemEntries = desktopRepository.LoadSystemEntries();
            var desktopSystemEntriesToDelete = postInstallDesktopSystemEntries.Except(preInstallDesktopSystemEntries).ToList();
            if (desktopSystemEntriesToDelete.Any())
            {
                desktopRepository.DeletePaths(desktopSystemEntriesToDelete);
            }

            consoleLogger.Result($"Installed '{app.AppId}'");
        }

        private async Task<bool> VerifyAppAsync(IApp app)
        {
            if (app.VerificationScript == null)
                return false;

            var verificationResult = await powerShell.ExecuteAsync(app.VerificationScript);
            return verificationResult.AsBool ?? false;
        }
    }
}
