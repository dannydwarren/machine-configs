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
    }

    public class Arguments : IArguments
    {
        public Arguments(string manifestPath,
            List<string> environments,
            string? downloadsDir = null)
        {
            ManifestPath = manifestPath;
            Environments = environments;
            DownloadsDir = downloadsDir ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        }

        public string ManifestPath { get; }
        public List<string> Environments { get; }
        public string DownloadsDir { get; }
    }
}
