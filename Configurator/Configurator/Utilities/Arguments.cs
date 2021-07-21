namespace Configurator.Utilities
{
    public interface IArguments
    {
        InstallEnvironment Environment { get; }
        string GitconfigsPath { get; }
        string WingetAppsPath { get; }
        string ScoopAppsPath { get; }
        string DownloadsDir { get; }
    }

    public class Arguments : IArguments
    {
        public Arguments(InstallEnvironment environment,
            string gitconfigsPath,
            string wingetAppsPath,
            string scoopAppsPath,
            string downloadsDir)
        {
            Environment = environment;
            GitconfigsPath = gitconfigsPath;
            WingetAppsPath = wingetAppsPath;
            ScoopAppsPath = scoopAppsPath;
            DownloadsDir = downloadsDir;
        }

        public InstallEnvironment Environment { get; }
        public string GitconfigsPath { get; }
        public string WingetAppsPath { get; }
        public string ScoopAppsPath { get; }
        public string DownloadsDir { get; }
    }
}
