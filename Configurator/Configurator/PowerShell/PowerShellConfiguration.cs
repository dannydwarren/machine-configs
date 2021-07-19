using Configurator.Utilities;
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
        private readonly IConsoleLogger consoleLogger;

        public PowerShellConfiguration(IPowerShell powerShell, IConsoleLogger consoleLogger)
        {
            this.powerShell = powerShell;
            this.consoleLogger = consoleLogger;
        }

        public async Task<string> SetExecutionPolicyAsync()
        {
            var result = await powerShell.ExecuteAsync(@"Set-ExecutionPolicy RemoteSigned
Get-ExecutionPolicy");
            consoleLogger.Result($"Execution Policy: {result.AsString}");

            return result.AsString;
        }
    }
}
