using System;
using System.Collections.Generic;
using System.IO;

namespace Configurator.Utilities
{
    public interface IArguments
    {
        string ManifestPath { get; }
        List<string> Environments { get; }
        string DownloadsDir { get; }
        string? SingleAppId { get; }
    }

    public class Arguments : IArguments
    {
        public static Arguments Default { get; }

        static Arguments()
        {
            Default = new Arguments(
                manifestPath: "https://raw.githubusercontent.com/dannydwarren/machine-configs/main/manifests/test.manifest.json",
                environments: new List<string> { "test" },
                downloadsDir: Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"));
        }


        public Arguments(string manifestPath,
            List<string> environments,
            string downloadsDir,
            string? singleAppId = null)
        {
            ManifestPath = manifestPath;
            Environments = environments;
            DownloadsDir = downloadsDir;
            SingleAppId = singleAppId;
        }

        public string ManifestPath { get; }
        public List<string> Environments { get; }
        public string DownloadsDir { get; }
        public string? SingleAppId { get; }
    }
}
