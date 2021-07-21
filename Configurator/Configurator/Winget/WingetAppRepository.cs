using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Configurator.Utilities;

namespace Configurator.Winget
{
    public interface IWingetAppRepository
    {
        Task<List<WingetApp>> LoadAsync();
    }

    public class WingetAppRepository : IWingetAppRepository
    {
        private readonly IArguments arguments;
        private readonly IFileSystem fileSystem;

        public WingetAppRepository(IArguments arguments, IFileSystem fileSystem)
        {
            this.arguments = arguments;
            this.fileSystem = fileSystem;
        }

        public async Task<List<WingetApp>> LoadAsync()
        {
            var rawApps = await fileSystem.ReadAllLinesAsync(arguments.WingetAppsPath);

            return rawApps.Select(ParseApp)
                .Where(x => x.Environment.HasFlag(arguments.Environment))
                .ToList();
        }

        private WingetApp ParseApp(string rawApp, int index)
        {
            var appLineNumber = index + 1;
            var parts = rawApp.Split(",", StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                throw new Exception($"Malformed winget app on line {appLineNumber} [missing parts]: {rawApp}");
            }

            var appId = parts[0];
            var environment = ParseEnvironment(parts[1], rawApp, appLineNumber);

            return new WingetApp
            {
                AppId = appId,
                Environment = environment
            };
        }

        private InstallEnvironment ParseEnvironment(string rawEnvironments, string rawApp, int appLineNumber)
        {
            return rawEnvironments.Split("|", StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                {
                    if (Enum.TryParse<InstallEnvironment>(x, out var environment))
                    {
                        return environment;
                    }
                    throw new Exception($"Malformed winget app on line {appLineNumber} [invalid install environment]: {rawApp}");
                })
                .Aggregate((environments, environment) => environments |= environment);
        }
    }

    public class WingetApp
    {
        public string AppId { get; set; } = "";
        public InstallEnvironment Environment { get; set; }
    }
}
