using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Configurator
{
    internal class Program
    {
        internal static async Task<int> Main(string[] args)
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

            var serviceCollection = new ServiceCollection();
            var dependencyBootstrapper = new DependencyBootstrapper(serviceCollection);
            var cli = new Cli(dependencyBootstrapper);

            rootCommand.Handler = CommandHandler.Create<string, List<string>, string>(cli.RunConfiguratorAsync);

            return await rootCommand.InvokeAsync(args);
        }
    }
}
