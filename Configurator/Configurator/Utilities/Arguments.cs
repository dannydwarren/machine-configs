namespace Configurator.Utilities
{
    public interface IArguments
    {
        InstallEnvironment Environment { get; }
        string ScoopAppsPath { get; }
        string GitconfigsPath { get; }
        string DownloadsDir { get; }
    }

    public class Arguments : IArguments
    {
        public Arguments(InstallEnvironment environment, string scoopAppsPath, string gitconfigsPath, string downloadsDir)
        {
            ScoopAppsPath = scoopAppsPath;
            Environment = environment;
            GitconfigsPath = gitconfigsPath;
            DownloadsDir = downloadsDir;
        }

        public InstallEnvironment Environment { get; }
        public string ScoopAppsPath { get; }
        public string GitconfigsPath { get; }
        public string DownloadsDir { get; }
    }
}
