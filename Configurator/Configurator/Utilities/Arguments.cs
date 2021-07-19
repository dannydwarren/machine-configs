namespace Configurator.Utilities
{
    public interface IArguments
    {
        InstallEnvironment Environment { get; }
        string ScoopAppsPath { get; }
        string GitconfigsPath { get; }
    }

    public class Arguments : IArguments
    {
        public Arguments(InstallEnvironment environment, string scoopAppsPath, string gitconfigsPath)
        {
            ScoopAppsPath = scoopAppsPath;
            Environment = environment;
            GitconfigsPath = gitconfigsPath;
        }

        public InstallEnvironment Environment { get; }
        public string ScoopAppsPath { get; }
        public string GitconfigsPath { get; }
    }
}
