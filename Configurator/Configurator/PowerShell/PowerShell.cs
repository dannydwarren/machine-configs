using Configurator.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

namespace Configurator.PowerShell
{
    public interface IPowerShell
    {
        Task<PowerShellResult> ExecuteAsync(string script);
        Task<PowerShellResult> ExecuteAsync(string script, string completeCheckScript);
    }

    /// <summary>
    /// Documentation: https://docs.microsoft.com/en-us/dotnet/api/system.management.automation.powershell?view=powershellsdk-7.0.0
    /// </summary>
    public class PowerShell : IPowerShell, IDisposable
    {
        private readonly IConsoleLogger consoleLogger;
        private readonly Runspace runspace;
        private readonly System.Management.Automation.PowerShell powershell;
        private bool disposedValue;

        public PowerShell(IConsoleLogger consoleLogger)
        {
            this.consoleLogger = consoleLogger;

            runspace = RunspaceFactory.CreateRunspace();
            powershell = System.Management.Automation.PowerShell.Create(runspace);
            runspace.Open();

            powershell.Streams.Debug.DataAdded += DebugDataAdded;
            powershell.Streams.Verbose.DataAdded += VerboseDataAdded;
            powershell.Streams.Information.DataAdded += InformationDataAdded;
            powershell.Streams.Warning.DataAdded += WarningDataAdded;
            powershell.Streams.Error.DataAdded += ErrorDataAdded;
            powershell.Streams.Progress.DataAdded += ProgressDataAdded;
        }

        public async Task<PowerShellResult> ExecuteAsync(string script)
        {
            return await ExecuteScriptAsync(script);
        }

        public async Task<PowerShellResult> ExecuteAsync(string script, string completeCheckScript)
        {
            var preCheckResult = await ExecuteScriptAsync(completeCheckScript);

            if (preCheckResult.AsBool ?? false)
            {
                return preCheckResult;
            }

            await ExecuteScriptAsync(script);

            return await ExecuteScriptAsync(completeCheckScript);
        }

        private async Task<PowerShellResult> ExecuteScriptAsync(string script)
        {
            consoleLogger.Debug(script);

            var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var currentUserCurrentHostProfile = Path.Combine(documentsFolder, "WindowsPowerShell\\Microsoft.PowerShell_profile.ps1");

            var environmentReadyScript = $@"
$env:Path = [System.Environment]::GetEnvironmentVariable(""Path"",""Machine"") + "";"" + [System.Environment]::GetEnvironmentVariable(""Path"",""User"")

if ($profile -eq $null -or $profile -eq '') {{
  $global:profile = ""{currentUserCurrentHostProfile}""
}}

{script}";

            powershell.AddScript(environmentReadyScript);

            var output = await powershell.InvokeAsync();

            var result = output.LastOrDefault();

            Clear();

            return new PowerShellResult
            {
                AsString = result?.ToString() ?? ""
            };
        }

        private void Clear()
        {
            powershell.Streams.ClearStreams();
            powershell.Commands.Clear();
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

            consoleLogger.Progress(
                $"{data.Activity} -> Status: {data.StatusDescription}; PercentComplete: {data.PercentComplete}");
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
        public string AsString { get; set; }
        public bool? AsBool => bool.TryParse(AsString, out var result) ? result : (bool?)null;
    }
}
