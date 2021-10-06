using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Configurator.UnitTests")]
[assembly: InternalsVisibleTo("Configurator.IntegrationTests")]
namespace Configurator.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.Scan(
                scan =>
                {
                    scan.FromAssembliesOf(typeof(DependencyInjectionConfig))
                        .AddClasses()
                        .AsSelf()
                        .AsMatchingInterface()
                        .WithTransientLifetime();
                }
            );

            Emmersion.Http.DependencyInjectionConfig.ConfigureServices(services);
        }
    }
}
