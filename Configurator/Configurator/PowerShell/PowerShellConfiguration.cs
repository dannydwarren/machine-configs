using Configurator.Utilities;
using System.Threading.Tasks;

namespace Configurator.PowerShell
{
    public interface IPowerShellConfiguration
    {
        Task SetExecutionPolicyAsync();
    }

    public class PowerShellConfiguration : IPowerShellConfiguration
    {
        private readonly IPowerShell powerShell;
        private readonly IWindowsPowerShell windowsPowerShell;
        private readonly IConsoleLogger consoleLogger;

        public PowerShellConfiguration(IPowerShell powerShell, IWindowsPowerShell windowsPowerShell, IConsoleLogger consoleLogger)
        {
            this.powerShell = powerShell;
            this.windowsPowerShell = windowsPowerShell;
            this.consoleLogger = consoleLogger;
        }

        public async Task SetExecutionPolicyAsync()
        {
            var executionPolicy = "RemoteSigned";
            var script = @$"Set-ExecutionPolicy {executionPolicy} -Force
Get-ExecutionPolicy";

            var powerShellResult = await powerShell.ExecuteAsync(script);
            consoleLogger.Result($"PowerShell - Execution Policy: {powerShellResult.AsString}");

            windowsPowerShell.Execute(script);
            consoleLogger.Result($"Windows PowerShell - Execution Policy: {executionPolicy}");
        }
    }
}
