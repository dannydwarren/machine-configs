using Configurator.Scoop;

namespace Configurator.Utilities
{
    public interface IArguments
    {
        string ScoopAppsPath { get; }
        InstallEnvironment Environment { get; }
    }

    public class Arguments : IArguments
    {
        public Arguments(string scoopAppsPath, InstallEnvironment environment)
        {
            ScoopAppsPath = scoopAppsPath;
            Environment = environment;
        }

        public string ScoopAppsPath { get; }

        public InstallEnvironment Environment { get; }
    }
}
