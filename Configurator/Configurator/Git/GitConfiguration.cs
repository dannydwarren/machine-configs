using Configurator.PowerShell;
using Configurator.Utilities;
using System.Threading.Tasks;

namespace Configurator.Git
{
    public interface IGitConfiguration
    {
        Task<bool> IncludeCustomGitconfigAsync(string gitconfigPath);
    }

    public class GitConfiguration : IGitConfiguration
    {
        private readonly IPowerShell powerShell;
        private readonly IConsoleLogger consoleLogger;

        public GitConfiguration(IPowerShell powerShell, IConsoleLogger consoleLogger)
        {
            this.powerShell = powerShell;
            this.consoleLogger = consoleLogger;
        }

        public async Task<bool> IncludeCustomGitconfigAsync(string gitconfigPath)
        {
            consoleLogger.Info("Including custom gitconfig");
            var result = await powerShell.ExecuteAsync(@$"
git config --global --add include.path {gitconfigPath}
(git config --get-all --global include.path) -match ""{gitconfigPath.Replace(@"\", @"\\")}""");
            consoleLogger.Result("Included custom gitconfig");

            return result.AsBool;
        }
    }
}
