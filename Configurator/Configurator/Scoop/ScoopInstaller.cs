using Configurator.Configuration;
using Configurator.PowerShell;
using System.Threading.Tasks;

namespace Configurator.Scoop
{
    public interface IScoopInstaller
    {
        Task InstallAsync(string appId);
    }

    public class ScoopInstaller : IScoopInstaller
    {
        private readonly IPowerShell powerShell;
        private readonly IConsoleLogger consoleLogger;

        public ScoopInstaller(IPowerShell powerShell, IConsoleLogger consoleLogger)
        {
            this.powerShell = powerShell;
            this.consoleLogger = consoleLogger;
        }

        public async Task InstallAsync(string appId)
        {
            consoleLogger.Info($"Installing '{appId}'");
            await powerShell.ExecuteAsync($"scoop install {appId}");
            consoleLogger.Result($"Installed '{appId}'");
        }
    }
}
