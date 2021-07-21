using System.Threading.Tasks;
using Configurator.PowerShell;
using Configurator.Utilities;

namespace Configurator.Winget
{
    public interface IWingetAppInstaller
    {
        Task InstallAsync(WingetApp app);
    }

    public class WingetAppInstaller : IWingetAppInstaller
    {
        private readonly IPowerShell powerShell;
        private readonly IConsoleLogger consoleLogger;

        public WingetAppInstaller(IPowerShell powerShell, IConsoleLogger consoleLogger)
        {
            this.powerShell = powerShell;
            this.consoleLogger = consoleLogger;
        }

        public async Task InstallAsync(WingetApp app)
        {
            consoleLogger.Info($"Installing '{app.AppId}'");
            await powerShell.ExecuteAsync($"winget install {app.AppId}");
            consoleLogger.Result($"Installed '{app.AppId}'");
        }
    }
}
