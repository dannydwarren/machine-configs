using System;
using System.Management.Automation.Runspaces;

namespace Configurator.PowerShell
{
    public interface IPowerShellConnection : IDisposable
    {
        System.Management.Automation.PowerShell Powershell { get; }
    }

    public class PowerShellConnection : IPowerShellConnection
    {
        private readonly Runspace runspace;
        private bool disposedValue;

        public PowerShellConnection()
        {
            runspace = RunspaceFactory.CreateRunspace();
            Powershell = System.Management.Automation.PowerShell.Create(runspace);
            runspace.Open();
        }

        public System.Management.Automation.PowerShell Powershell { get; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    runspace.Dispose();
                    Powershell.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
