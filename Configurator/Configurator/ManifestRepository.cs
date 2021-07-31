using System;
using System.Linq;
using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.Configuration;
using Configurator.Utilities;

namespace Configurator
{
    public interface IManifestRepository
    {
        Task<Manifest> LoadAsync();
    }

    public class ManifestRepository : IManifestRepository
    {
        private readonly IArguments arguments;
        private readonly IFileSystem fileSystem;
        private readonly IJsonSerializer jsonSerializer;
        private readonly IResourceDownloader resourceDownloader;

        public ManifestRepository(IArguments arguments,
            IFileSystem fileSystem,
            IJsonSerializer jsonSerializer,
            IResourceDownloader resourceDownloader)
        {
            this.arguments = arguments;
            this.fileSystem = fileSystem;
            this.jsonSerializer = jsonSerializer;
            this.resourceDownloader = resourceDownloader;
        }

        public async Task<Manifest> LoadAsync()
        {
            Console.WriteLine($"ManifestPath: {arguments.ManifestPath}");
            var manifestJson = await ReadManifestAsync();
            var manifest = jsonSerializer.Deserialize<Manifest>(manifestJson)!;

            return new Manifest
            {
                WingetApps = manifest.WingetApps.Where(IsForEnvironment).ToList(),
                ScoopBuckets = manifest.ScoopBuckets.Where(IsForEnvironment).ToList(),
                ScoopApps = manifest.ScoopApps.Where(IsForEnvironment).ToList(),
                NonPackageApps = manifest.NonPackageApps.Where(IsForEnvironment).ToList(),
                PowerShellAppPackages = manifest.PowerShellAppPackages.Where(IsForEnvironment).ToList(),
                Gitconfigs = manifest.Gitconfigs.Where(IsForEnvironment).ToList(),
            };
        }

        private async Task<string> ReadManifestAsync()
        {
            var manifestFilePath = arguments.ManifestPath;
            if (arguments.ManifestPath.StartsWith("http"))
            {
                manifestFilePath = await resourceDownloader.DownloadAsync(arguments.ManifestPath,
                    GetFileNameFromHttpResource(arguments.ManifestPath));
            }

            return await fileSystem.ReadAllTextAsync(manifestFilePath);
        }

        private string GetFileNameFromHttpResource(string httpManifestPath)
        {
            var lastSlash = httpManifestPath.LastIndexOf("/", StringComparison.Ordinal);
            return httpManifestPath[(lastSlash + 1)..];
        }

        private bool IsForEnvironment(IApp app)
        {
            return arguments.Environments.Any(x => x.ToLower() == "all")
                   || app.Environments.ToLower().Contains("all")
                   || arguments.Environments.Any(x => app.Environments.ToLower().Contains(x.ToLower()));
        }
    }
}
