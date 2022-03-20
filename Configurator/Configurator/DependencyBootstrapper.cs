using System;
using System.Reflection;
using System.Threading.Tasks;
using Configurator.Configuration;
using Configurator.PowerShell;
using Configurator.Utilities;
using Configurator.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Configurator
{
    public interface IDependencyBootstrapper
    {
        Task<IServiceProvider> InitializeAsync(IArguments arguments);
    }

    public class DependencyBootstrapper : IDependencyBootstrapper
    {
        private readonly IServiceCollection serviceCollection;

        public DependencyBootstrapper(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
        }

        public async Task<IServiceProvider> InitializeAsync(IArguments arguments)
        {
            var serviceProvider = InitializeServiceProvider(arguments);
            InitializeStaticDependencies(serviceProvider);

            await WriteDependencyDebugInfo(serviceProvider);

            return serviceProvider;
        }

        /// <summary>
        /// Not for public consumption! Only exposed for unit testing!
        /// </summary>
        internal ServiceProvider InitializeServiceProvider(IArguments arguments)
        {
            DependencyInjectionConfig.ConfigureServices(serviceCollection);
            serviceCollection.AddSingleton(arguments);

            return serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Not for public consumption! Only exposed for unit testing!
        /// </summary>
        internal void InitializeStaticDependencies(IServiceProvider services)
        {
            RegistrySettingValueDataConverter.Tokenizer = services.GetRequiredService<ITokenizer>();
        }

        private static async Task WriteDependencyDebugInfo(IServiceProvider services)
        {
            var logger = services.GetRequiredService<IConsoleLogger>();

            var version = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly())
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
                .InformationalVersion;
            logger.Debug($"{nameof(Configurator)} version: {version}");

            var powerShell = services.GetRequiredService<IPowerShell>();
            var result = await powerShell.ExecuteAsync("$PSVersionTable.PSVersion.ToString()");
            logger.Debug($"PowerShell Version: {result.AsString}");

            var args = services.GetRequiredService<IArguments>();
            logger.Debug($@"{nameof(IArguments)}:
{{
  {nameof(args.ManifestPath)} = ""{args.ManifestPath}""
  {nameof(args.Environments)} = ""{string.Join("|", args.Environments)}""
  {nameof(args.DownloadsDir)} = ""{args.DownloadsDir}""
  {nameof(args.SingleAppId)} = ""{args.SingleAppId}""
}}");
        }
    }
}
