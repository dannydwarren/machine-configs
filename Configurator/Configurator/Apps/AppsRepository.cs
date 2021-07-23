using System.Linq;
using System.Threading.Tasks;
using Configurator.Configuration;
using Configurator.Utilities;

namespace Configurator.Apps
{
    public interface IAppsRepository
    {
        Task<Apps> LoadAsync();
    }

    public class AppsRepository : IAppsRepository
    {
        private readonly IArguments arguments;
        private readonly IFileSystem fileSystem;
        private readonly IJsonSerializer jsonSerializer;

        public AppsRepository(IArguments arguments,
            IFileSystem fileSystem,
            IJsonSerializer jsonSerializer)
        {
            this.arguments = arguments;
            this.fileSystem = fileSystem;
            this.jsonSerializer = jsonSerializer;
        }

        public async Task<Apps> LoadAsync()
        {
            var appsJson = await fileSystem.ReadAllTextAsync(arguments.AppsPath);
            var apps = jsonSerializer.Deserialize<Apps>(appsJson)!;

            return new Apps
            {
                WingetApps = apps.WingetApps.Where(IsForEnvironment).ToList(),
                ScoopApps = apps.ScoopApps.Where(IsForEnvironment).ToList(),
                NonPackageApps = apps.NonPackageApps.Where(IsForEnvironment).ToList()
            };
        }

        private bool IsForEnvironment(IApp app)
        {
            return arguments.Environments.Any(x => x.ToLower() == "all")
                   || arguments.Environments.Any(x => app.Environments.Contains(x));
        }
    }
}
