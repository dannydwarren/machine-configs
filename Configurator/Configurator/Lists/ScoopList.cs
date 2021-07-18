using Configurator.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Configurator.Lists
{
    public interface IScoopList
    {
        Task<List<ScoopApp>> LoadAsync();
    }

    public class ScoopList : IScoopList
    {
        private readonly IArguments arguments;
        private readonly IFileSystem fileSystem;

        public ScoopList(IArguments arguments, IFileSystem fileSystem)
        {
            this.arguments = arguments;
            this.fileSystem = fileSystem;
        }

        public async Task<List<ScoopApp>> LoadAsync()
        {
            var rawApps = await fileSystem.ReadAllLinesAsync(arguments.ScoopAppsPath);

            return rawApps.Select(ParseApp).ToList();
        }

        private ScoopApp ParseApp(string rawApp, int index)
        {
            var appLineNumber = index + 1;
            var parts = rawApp.Split(",", StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                throw new Exception($"Malformed scoop app on line {appLineNumber} [missing parts]: {rawApp}");
            }

            var appId = parts[0];
            var environment = ParseEnvironment(parts[1], rawApp, appLineNumber);

            return new ScoopApp
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
                     throw new Exception($"Malformed scoop app on line {appLineNumber} [invalid install environment]: {rawApp}");
                 })
                 .Aggregate((environments, environment) => environments |= environment);
        }
    }

    public class ScoopApp
    {
        public string AppId { get; set; } = "";
        public InstallEnvironment Environment { get; set; }
    }

    [Flags]
    public enum InstallEnvironment
    {
        Personal = 1 << 0,
        Work = 1 << 1,
        All = Personal | Work
    }
}
