using System.Collections.Generic;

namespace Configurator.Utilities
{
    public interface IArguments
    {
        string ManifestPath { get; }
        string DownloadsDir { get; }
        List<string> Environments { get; }
    }

    public class Arguments : IArguments
    {
        public Arguments(string manifestPath,
            string downloadsDir,
            List<string> environments)
        {
            ManifestPath = manifestPath;
            DownloadsDir = downloadsDir;
            Environments = environments;
        }

        public string ManifestPath { get; }
        public string DownloadsDir { get; }
        public List<string> Environments { get; }
    }
}
