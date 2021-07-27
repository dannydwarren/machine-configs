using System.Threading.Tasks;
using Configurator.PowerShell;

namespace Configurator
{
    public interface ISystemInitializer
    {
        Task InitializeAsync();
    }

    public class SystemInitializer : ISystemInitializer
    {
        private readonly IPowerShellConfiguration powerShellConfiguration;

        public SystemInitializer(IPowerShellConfiguration powerShellConfiguration)
        {
            this.powerShellConfiguration = powerShellConfiguration;
        }

        public async Task InitializeAsync()
        {
            await powerShellConfiguration.SetExecutionPolicyAsync();
        }
    }
}
