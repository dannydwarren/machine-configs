using System.Collections.Generic;

namespace Configurator.Utilities
{
    public interface IArguments
    {
        InstallEnvironment Environment { get; }
        string AppsPath { get; }
        string GitconfigsPath { get; }
        string DownloadsDir { get; }
        List<string> Environments { get; }
    }

    public class Arguments : IArguments
    {
        public Arguments(InstallEnvironment environment,
            string appsPath,
            string gitconfigsPath,
            string downloadsDir,
            List<string> environments)
        {
            Environment = environment;
            AppsPath = appsPath;
            GitconfigsPath = gitconfigsPath;
            DownloadsDir = downloadsDir;
            Environments = environments;
        }

        public InstallEnvironment Environment { get; }
        public string AppsPath { get; }
        public string GitconfigsPath { get; }
        public string DownloadsDir { get; }
        public List<string> Environments { get; }
    }
}
