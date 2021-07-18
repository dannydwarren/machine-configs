using Configurator.PowerShell;
using System.Threading.Tasks;

namespace Configurator.Installers
{
    public interface IScoopInstaller
    {
        Task InstallAsync(string appId);
    }

    public class ScoopInstaller : IScoopInstaller
    {
        private readonly IPowerShell powerShell;

        public ScoopInstaller(IPowerShell powerShell)
        {
            this.powerShell = powerShell;
        }

        public async Task InstallAsync(string appId)
        {
            await powerShell.ExecuteAsync($"scoop install {appId}");
        }
    }
}
