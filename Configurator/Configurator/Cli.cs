using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading.Tasks;
using Configurator.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Configurator
{
    public class Cli
    {
        private readonly IDependencyBootstrapper dependencyBootstrapper;

        public Cli(IDependencyBootstrapper dependencyBootstrapper)
        {
            this.dependencyBootstrapper = dependencyBootstrapper;
        }

        public async Task<int> LaunchAsync(params string[] args)
        {
                var rootCommand = new RootCommand
                {
                    new Option<string>(
                        aliases: new[] { "--manifest-path", "-m" },
                        getDefaultValue: () => Arguments.Default.ManifestPath
                        // description: "Path (local or URL) to your manifest file."
                        ),
                    new Option<List<string>>(
                        aliases: new[] { "--environments", "-e" },
                        parseArgument: x =>
                            x.Tokens.FirstOrDefault()?.Value.Split("|", StringSplitOptions.RemoveEmptyEntries).ToList()
                                ?? Arguments.Default.Environments,
                        isDefault: true
                        // description: "Pipe separated list of environments to target in the manifest."
                        ),
                    new Option<string>(
                        aliases: new[] { "--downloads-dir", "-dl" },
                        getDefaultValue: () => Arguments.Default.DownloadsDir
                        // description: "Path (local or URL) to your manifest file."
                    )
                };

                rootCommand.Description = "Configurator";

                var cli = new Cli(dependencyBootstrapper);

                rootCommand.Handler = CommandHandler.Create<string, List<string>, string>(cli.RunConfiguratorAsync);

                return await rootCommand.InvokeAsync(args);
        }

        public async Task RunConfiguratorAsync(string? manifestPath = null, List<string>? environments = null, string? downloadsDir = null)
        {
            var services = await dependencyBootstrapper.InitializeAsync(manifestPath, environments, downloadsDir);
            var configurator = services.GetRequiredService<IMachineConfigurator>();

            await configurator.ExecuteAsync();
        }
    }
}
