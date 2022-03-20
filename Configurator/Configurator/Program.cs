using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Configurator
{
    internal static class Program
    {
        internal static async Task<int> Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            var dependencyBootstrapper = new DependencyBootstrapper(serviceCollection);
            var cli = new Cli(dependencyBootstrapper);

            return await cli.LaunchAsync(args);
        }
    }
}
