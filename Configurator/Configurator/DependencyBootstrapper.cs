using System;
using System.Collections.Generic;
using System.IO;
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
        Task<IServiceProvider> InitializeAsync(string? manifestPath = null, List<string>? environments = null, string? downloadsDir = null);
    }

    public class DependencyBootstrapper : IDependencyBootstrapper
    {
        private readonly IServiceCollection serviceCollection;

        public DependencyBootstrapper(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
        }

        public async Task<IServiceProvider> InitializeAsync(string? manifestPath = null, List<string>? environments = null, string? downloadsDir = null)
        {
            var serviceProvider = InitializeServiceProvider(manifestPath, environments, downloadsDir);
            InitializeStaticDependencies(serviceProvider);

            await WriteDependencyDebugInfo(serviceProvider);

            return serviceProvider;
        }

        /// <summary>
        /// Not for public consumption! Only exposed for unit testing!
        /// </summary>
        internal ServiceProvider InitializeServiceProvider(string? manifestPath = null, List<string>? environments = null, string? downloadsDir = null)
        {
            var arguments = new Arguments(
                manifestPath: manifestPath ?? "https://raw.githubusercontent.com/dannydwarren/machine-configs/main/manifests/test.manifest.json",
                environments: environments ?? new List<string>{ "test" },
                downloadsDir: downloadsDir ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"));

            DependencyInjectionConfig.ConfigureServices(serviceCollection);
            serviceCollection.AddSingleton<IArguments>(arguments);

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
  {nameof(args.DownloadsDir)} = ""{string.Join("|", args.DownloadsDir)}""
}}");
        }
    }
}
