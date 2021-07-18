using Configurator.Configuration;
using System;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

namespace Configurator.PowerShell
{
    public interface IPowerShell
    {
        Task<PowerShellResult> ExecuteAsync(string script);
    }

    /// <summary>
    /// Documentation: https://docs.microsoft.com/en-us/dotnet/api/system.management.automation.powershell?view=powershellsdk-7.0.0
    /// </summary>
    public class PowerShell : IPowerShell, IDisposable
    {
        private readonly IConsoleLogger consoleLogger;
        private readonly System.Management.Automation.PowerShell powershell;
        private readonly Runspace runspace;
        private bool disposedValue;

        public PowerShell(IConsoleLogger consoleLogger)
        {
            this.consoleLogger = consoleLogger;

            runspace = RunspaceFactory.CreateRunspace();
            powershell = System.Management.Automation.PowerShell.Create(runspace);

            powershell.Streams.Debug.DataAdded += DebugDataAdded;
            powershell.Streams.Verbose.DataAdded += VerboseDataAdded;
            powershell.Streams.Information.DataAdded += InformationDataAdded;
            powershell.Streams.Warning.DataAdded += WarningDataAdded;
            powershell.Streams.Error.DataAdded += ErrorDataAdded;
            powershell.Streams.Progress.DataAdded += ProgressDataAdded;
        }

        public async Task<PowerShellResult> ExecuteAsync(string script)
        {
            runspace.Open();

            powershell.AddScript(script);

            var output = await powershell.InvokeAsync();

            var result = output.LastOrDefault();

            return new PowerShellResult
            {
                AsString = result?.ToString() ?? ""
            };
        }

        private void DebugDataAdded(object? sender, System.Management.Automation.DataAddedEventArgs e)
        {
            var data = powershell.Streams.Debug[e.Index];

            consoleLogger.Debug(data.Message);
        }

        private void VerboseDataAdded(object? sender, System.Management.Automation.DataAddedEventArgs e)
        {
            var data = powershell.Streams.Verbose[e.Index];

            consoleLogger.Verbose(data.Message);
        }

        private void InformationDataAdded(object? sender, System.Management.Automation.DataAddedEventArgs e)
        {
            var data = powershell.Streams.Information[e.Index];

            if (data.MessageData != null)
            {
                consoleLogger.Info(data.MessageData.ToString()!);
            }
        }

        private void WarningDataAdded(object? sender, System.Management.Automation.DataAddedEventArgs e)
        {
            var data = powershell.Streams.Warning[e.Index];

            consoleLogger.Warn(data.Message);
        }

        private void ErrorDataAdded(object? sender, System.Management.Automation.DataAddedEventArgs e)
        {
            var data = powershell.Streams.Error[e.Index];

            consoleLogger.Error(data.ToString());
        }

        private void ProgressDataAdded(object? sender, System.Management.Automation.DataAddedEventArgs e)
        {
            var data = powershell.Streams.Progress[e.Index];

            consoleLogger.Progress($"{data.Activity} -> Status: {data.StatusDescription}; PercentComplete: {data.PercentComplete}");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    runspace.Dispose();
                    powershell.Dispose();
                }

                powershell.Streams.Debug.DataAdded -= DebugDataAdded;
                powershell.Streams.Verbose.DataAdded -= VerboseDataAdded;
                powershell.Streams.Information.DataAdded -= InformationDataAdded;
                powershell.Streams.Warning.DataAdded -= WarningDataAdded;
                powershell.Streams.Error.DataAdded -= ErrorDataAdded;
                powershell.Streams.Progress.DataAdded -= ProgressDataAdded;
                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class PowerShellResult
    {
        public string AsString { get; set; } = "";
    }
}
