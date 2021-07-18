using System;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

namespace Configurator.PowerShell
{
    public interface IPowerShell
    {
        Task<string> ExecuteAsync(string script);
    }

    public class PowerShell : IPowerShell
    {
        public async Task<string> ExecuteAsync(string script)
        {
            using var runspace = RunspaceFactory.CreateRunspace();
            using var powershell = System.Management.Automation.PowerShell.Create(runspace);
            runspace.Open();
            powershell.AddScript(script);

            var result = await powershell.InvokeAsync();

            var objects = result.ReadAll();
            for(var i = 0; i < objects.Count; i++)
            {
                Console.WriteLine($"Item: {i}");
                Console.WriteLine(objects[i].ToString());
                Console.WriteLine(Environment.NewLine+Environment.NewLine);
            }

            if (powershell.Streams.Debug.Any())
            {
                Console.WriteLine($"Debug:{Environment.NewLine}{string.Join(Environment.NewLine+Environment.NewLine, powershell.Streams.Debug.Select(x => x.Message))}");
            }
            if (powershell.Streams.Information.Any())
            {
                Console.WriteLine($"Information:{Environment.NewLine}{string.Join(Environment.NewLine + Environment.NewLine, powershell.Streams.Information.Select(x => x.MessageData))}");
            }
            if (powershell.Streams.Warning.Any())
            {
                Console.WriteLine($"Warning:{Environment.NewLine}{string.Join(Environment.NewLine + Environment.NewLine, powershell.Streams.Warning.Select(x => x.Message))}");
            }
            if (powershell.Streams.Error.Any())
            {
                throw new Exception($"{nameof(PowerShell)} - Error occured while executing script \"{script}\".{Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine + Environment.NewLine, powershell.Streams.Error.Select(x => x.Exception.ToString()))}");
            }

            return "";
        }
    }
}
