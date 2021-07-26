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

        public ManifestRepository(IArguments arguments,
            IFileSystem fileSystem,
            IJsonSerializer jsonSerializer)
        {
            this.arguments = arguments;
            this.fileSystem = fileSystem;
            this.jsonSerializer = jsonSerializer;
        }

        public async Task<Manifest> LoadAsync()
        {
            var manifestJson = await fileSystem.ReadAllTextAsync(arguments.ManifestPath);
            var manifest = jsonSerializer.Deserialize<Manifest>(manifestJson)!;

            return new Manifest
            {
                WingetApps = manifest.WingetApps.Where(IsForEnvironment).ToList(),
                ScoopApps = manifest.ScoopApps.Where(IsForEnvironment).ToList(),
                NonPackageApps = manifest.NonPackageApps.Where(IsForEnvironment).ToList(),
                PowerShellAppPackages = manifest.PowerShellAppPackages.Where(IsForEnvironment).ToList()
            };
        }

        private bool IsForEnvironment(IApp app)
        {
            return arguments.Environments.Any(x => x.ToLower() == "all")
                   || app.Environments.ToLower().Contains("all")
                   || arguments.Environments.Any(x => app.Environments.ToLower().Contains(x.ToLower()));
        }
    }
}
