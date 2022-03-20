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
                    ),
                    new Option<string>(
                        aliases: new[] { "--single-app-id", "-app" }
                        // description: "Specify the single app to install. When present the environments arg is ignored."
                    )
                };

                rootCommand.Description = "Configurator";

                var cli = new Cli(dependencyBootstrapper);

                rootCommand.Handler = CommandHandler.Create<string, List<string>, string, string>(cli.RunConfiguratorAsync);

                return await rootCommand.InvokeAsync(args);
        }

        private async Task RunConfiguratorAsync(string manifestPath, List<string> environments, string downloadsDir, string? singleAppId)
        {
            var arguments = new Arguments(manifestPath, environments, downloadsDir, singleAppId);

            var services = await dependencyBootstrapper.InitializeAsync(arguments);
            var configurator = services.GetRequiredService<IMachineConfigurator>();

            await configurator.ExecuteAsync();
        }
    }
}
