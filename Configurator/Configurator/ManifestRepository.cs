using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
            var manifestJson = await ReadManifestAsync();
            var manifest = jsonSerializer.Deserialize<RawManifest>(manifestJson)!;
            var installables = manifest.Apps.Select(x =>
            {
                var installable = jsonSerializer.Deserialize<Installable>(x.ToString()!);
                installable.AppData = x;

                return installable;
            }).ToList();
            var installablesToInstall = installables.Where(IsIncluded).ToList();

            return new Manifest
            {
                Apps = ParseApps(installablesToInstall)
            };
        }

        private List<IApp> ParseApps(List<Installable> installables)
        {
            return installables.Select(x => (IApp)(x switch
            {
                { AppType: AppType.Gitconfig } => jsonSerializer.Deserialize<GitconfigApp>(x.AppData.ToString()!),
                { AppType: AppType.NonPackageApp } => jsonSerializer.Deserialize<NonPackageApp>(x.AppData.ToString()!),
                { AppType: AppType.PowerShellAppPackage } => jsonSerializer.Deserialize<PowerShellAppPackage>(x.AppData.ToString()!),
                { AppType: AppType.PowerShellModule } => jsonSerializer.Deserialize<PowerShellModuleApp>(x.AppData.ToString()!),
                { AppType: AppType.Scoop } => jsonSerializer.Deserialize<ScoopApp>(x.AppData.ToString()!),
                { AppType: AppType.ScoopBucket } => jsonSerializer.Deserialize<ScoopBucketApp>(x.AppData.ToString()!),
                { AppType: AppType.Script } => jsonSerializer.Deserialize<ScriptApp>(x.AppData.ToString()!),
                { AppType: AppType.VisualStudioExtension } => jsonSerializer.Deserialize<VisualStudioExtensionApp>(x.AppData.ToString()!),
                { AppType: AppType.Winget } => jsonSerializer.Deserialize<WingetApp>(x.AppData.ToString()!),
                _ => null!
            })).Where(x => x != null).ToList()!;
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

        private bool IsIncluded(Installable installable)
        {
            return arguments.SingleAppId != null
                ? installable.AppId == arguments.SingleAppId
                : IsForEnvironment(installable);
        }

        private bool IsForEnvironment(Installable installable)
        {
            return arguments.Environments.Any(x => x.ToLower() == "all")
                   || installable.Environments.ToLower().Contains("all")
                   || arguments.Environments.Any(x => installable.Environments.ToLower().Contains(x.ToLower()));
        }

        internal class RawManifest
        {
            public List<JsonElement> Apps { get; set; } = new List<JsonElement>();
        }

        internal class Installable
        {
            public string AppId { get; set; }
            public AppType AppType { get; set; }
            public string Environments { get; set; }
            public JsonElement AppData { get; set; }
        }
    }
}
