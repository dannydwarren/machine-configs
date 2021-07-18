using System.Threading.Tasks;

namespace Configurator.PowerShell
{
    public interface IPowerShellConfiguration
    {
        Task<string> SetExecutionPolicyAsync();
    }

    public class PowerShellConfiguration : IPowerShellConfiguration
    {
        private readonly IPowerShell powerShell;

        public PowerShellConfiguration(IPowerShell powerShell)
        {
            this.powerShell = powerShell;
        }

        public async Task<string> SetExecutionPolicyAsync()
        {
            return await powerShell.ExecuteAsync("Set-ExecutionPolicy RemoteSigned");
        }
    }
}
