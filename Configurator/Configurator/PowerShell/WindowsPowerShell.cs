using System.Diagnostics;

namespace Configurator.PowerShell
{
    public interface IWindowsPowerShell
    {
        void Execute(string script);
    }

    public class WindowsPowerShell : IWindowsPowerShell
    {
        public void Execute(string script)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    RedirectStandardOutput = false,
                    Verb = "runas",
                    FileName = @"C:\windows\system32\windowspowershell\v1.0\powershell.exe",
                    Arguments = @$"""{script}"""
                },
                EnableRaisingEvents = true
            };

            process.Start();
            process.WaitForExit();
        }
    }
}
