using System.Threading.Tasks;
using Configurator.PowerShell;
using Configurator.Utilities;

namespace Configurator.Apps
{
    public interface IAppInstaller
    {
        Task InstallAsync(IApp app);
    }

    public class AppInstaller : IAppInstaller
    {
        private readonly IPowerShell powerShell;
        private readonly IConsoleLogger consoleLogger;

        public AppInstaller(IPowerShell powerShell, IConsoleLogger consoleLogger)
        {
            this.powerShell = powerShell;
            this.consoleLogger = consoleLogger;
        }

        public async Task InstallAsync(IApp app)
        {
            consoleLogger.Info($"Installing '{app.AppId}'");
            await powerShell.ExecuteAsync(app.InstallScript);
            consoleLogger.Result($"Installed '{app.AppId}'");
        }
    }
}
