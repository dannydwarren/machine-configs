using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.Configuration;
using Configurator.Utilities;

namespace Configurator
{
    public interface IManifestRepositoryV2
    {
        Task<ManifestV2> LoadAsync();
    }

    public class ManifestRepositoryV2 : IManifestRepositoryV2
    {
        private readonly IArguments arguments;
        private readonly IFileSystem fileSystem;
        private readonly IJsonSerializer jsonSerializer;
        private readonly IResourceDownloader resourceDownloader;

        public ManifestRepositoryV2(IArguments arguments,
            IFileSystem fileSystem,
            IJsonSerializer jsonSerializer,
            IResourceDownloader resourceDownloader)
        {
            this.arguments = arguments;
            this.fileSystem = fileSystem;
            this.jsonSerializer = jsonSerializer;
            this.resourceDownloader = resourceDownloader;
        }

        public async Task<ManifestV2> LoadAsync()
        {
            Console.WriteLine($"ManifestPath: {arguments.ManifestPath}");
            var manifestJson = await ReadManifestAsync();
            var manifest = jsonSerializer.Deserialize<ManifestV2Raw>(manifestJson)!;
            var installables = manifest.Apps.Select(x =>
            {
                var installable = jsonSerializer.Deserialize<Installable>(x.ToString()!);
                installable.AppData = x;

                return installable;
            }).ToList();
            var installablesToInstall = installables.Where(IsForEnvironment).ToList();

            return new ManifestV2
            {
                Apps = ParseApps(installablesToInstall)
            };
        }

        private List<IApp> ParseApps(List<Installable> installables)
        {
            return installables.Select(x => (IApp)(x switch
            {
                { Installer: Installer.Gitconfig } => jsonSerializer.Deserialize<GitconfigApp>(x.AppData.ToString()!),
                { Installer: Installer.NonPackageApp } => jsonSerializer.Deserialize<NonPackageApp>(x.AppData.ToString()!),
                { Installer: Installer.PowerShellAppPackage } => jsonSerializer.Deserialize<PowerShellAppPackage>(x.AppData.ToString()!),
                { Installer: Installer.Scoop } => jsonSerializer.Deserialize<ScoopApp>(x.AppData.ToString()!),
                { Installer: Installer.ScoopBucket } => jsonSerializer.Deserialize<ScoopBucketApp>(x.AppData.ToString()!),
                { Installer: Installer.Script } => jsonSerializer.Deserialize<ScriptApp>(x.AppData.ToString()!),
                { Installer: Installer.Winget } => jsonSerializer.Deserialize<WingetApp>(x.AppData.ToString()!),
                _ => throw new Exception($"Installer type '{x.Installer}' not supported")
            })).ToList();
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

        private bool IsForEnvironment(IInstallable installable)
        {
            return arguments.Environments.Any(x => x.ToLower() == "all")
                   || installable.Environments.ToLower().Contains("all")
                   || arguments.Environments.Any(x => installable.Environments.ToLower().Contains(x.ToLower()));
        }
    }
}
