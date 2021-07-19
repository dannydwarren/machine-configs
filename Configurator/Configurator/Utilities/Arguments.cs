using Configurator.Scoop;

namespace Configurator.Utilities
{
    public interface IArguments
    {
        InstallEnvironment Environment { get; }
        string ScoopAppsPath { get; }
        string GitconfigPath { get; }
    }

    public class Arguments : IArguments
    {
        public Arguments(InstallEnvironment environment, string scoopAppsPath, string gitconfigPath)
        {
            ScoopAppsPath = scoopAppsPath;
            Environment = environment;
            GitconfigPath = gitconfigPath;
        }

        public InstallEnvironment Environment { get; }
        public string ScoopAppsPath { get; }
        public string GitconfigPath { get; }
    }
}
