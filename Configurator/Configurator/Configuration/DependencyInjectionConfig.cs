using Configurator.PowerShell;
using Configurator.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Configurator.Configuration
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

            services.AddSingleton<IPowerShell>(x => new PowerShell.PowerShell(x.GetRequiredService<IConsoleLogger>()));
        }
    }
}
