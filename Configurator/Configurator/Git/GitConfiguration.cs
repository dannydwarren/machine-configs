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
            var script = @$"git config --global --add include.path {gitconfigPath}";
            var completeCheckScript = @$"(git config --get-all --global include.path) -match ""{gitconfigPath.Replace(@"\", @"\\")}""";

            consoleLogger.Info("Including custom gitconfig");
            var result = await powerShell.ExecuteAsync(script, completeCheckScript);
            consoleLogger.Result("Included custom gitconfig");

            return result.AsBool ?? false;
        }
    }
}
