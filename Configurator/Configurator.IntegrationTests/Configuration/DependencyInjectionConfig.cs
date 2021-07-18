using Microsoft.Extensions.DependencyInjection;

namespace Configurator.IntegrationTests.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.Scan(
                scan =>
                {
                    scan.FromAssembliesOf(typeof(DependencyInjectionConfig)).AddClasses().AsMatchingInterface().WithTransientLifetime();
                }
            );

            Configurator.Configuration.DependencyInjectionConfig.ConfigureServices(services);
        }
    }
}
