using System.Linq;
using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.PowerShell;
using Configurator.Utilities;

namespace Configurator.Installers
{
    public interface IAppInstaller
    {
        Task InstallAsync(IApp app);
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

        public async Task InstallAsync(IApp app)
        {
            consoleLogger.Info($"Installing '{app.AppId}'");
            var preInstallDesktopSystemEntries = desktopRepository.LoadSystemEntries();
            await powerShell.ExecuteAsync(app.InstallScript);
            var postInstallDesktopSystemEntries = desktopRepository.LoadSystemEntries();
            desktopRepository.DeletePaths(postInstallDesktopSystemEntries.Except(preInstallDesktopSystemEntries).ToList());
            consoleLogger.Result($"Installed '{app.AppId}'");
        }
    }
}
