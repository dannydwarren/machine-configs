using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Reflection;
using Configurator.Configuration;
using Configurator.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Configurator.PowerShell;
using Configurator.Windows;

namespace Configurator
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option<string>(
                    aliases: new[] { "--manifest-path", "-m" },
                    getDefaultValue: () =>
                        "https://raw.githubusercontent.com/dannydwarren/machine-configs/main/manifests/test.manifest.json",
                    description: "Path (local or URL) to your manifest file."),
                new Option<List<string>>(
                    aliases: new[] { "--environments", "-e" },
                    parseArgument: x =>
                        x.Tokens.FirstOrDefault()?.Value.Split("|", StringSplitOptions.RemoveEmptyEntries).ToList()
                        ?? new List<string> { "Test" },
                    isDefault: true,
                    description: "Pipe separated list of environments to target in the manifest.")
            };

            rootCommand.Description = "Configurator";

            rootCommand.Handler = CommandHandler.Create<string, List<string>>(Run);

            return await rootCommand.InvokeAsync(args);
        }

        private static async Task Run(string manifestPath, List<string> environments)
        {
            var serviceCollection = new ServiceCollection();
            var dependencyBootstrapper = new DependencyBootstrapper(serviceCollection);
            var services = await dependencyBootstrapper.InitializeAsync();

            var config = services.GetRequiredService<IMachineConfigurator>();

            await config.ExecuteAsync();

            await ((IAsyncDisposable)services).DisposeAsync();
        }
    }
}
