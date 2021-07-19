using Configurator.Scoop;
using Configurator.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Configurator.Git
{
    public interface IGitconfigRepository
    {
        Task<List<Gitconfig>> LoadAsync();
    }

    public class GitconfigRepository : IGitconfigRepository
    {
        private readonly IArguments arguments;
        private readonly IFileSystem fileSystem;

        public GitconfigRepository(IArguments arguments, IFileSystem fileSystem)
        {
            this.arguments = arguments;
            this.fileSystem = fileSystem;
        }

        public async Task<List<Gitconfig>> LoadAsync()
        {
            var csv = await fileSystem.ReadAllLinesAsync(arguments.GitconfigsPath);

            return csv.Select(ParseGitconfig)
                .Where(x => x.Environment.HasFlag(arguments.Environment))
                .ToList();
        }

        private Gitconfig ParseGitconfig(string rawGitconfig, int index)
        {
            var lineNumber = index + 1;
            var parts = rawGitconfig.Split(",", StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                throw new Exception($"Malformed gitconfig on line {lineNumber} [missing parts]: {rawGitconfig}");
            }

            var gitconfigPath = parts[0];
            var environment = ParseEnvironment(parts[1], rawGitconfig, lineNumber);


            return new Gitconfig
            {
                Path = gitconfigPath,
                Environment = environment
            };
        }

        private InstallEnvironment ParseEnvironment(string rawEnvironments, string rawGitconfig, int lineNumber)
        {
            return rawEnvironments.Split("|", StringSplitOptions.RemoveEmptyEntries)
                 .Select(x =>
                 {
                     if (Enum.TryParse<InstallEnvironment>(x, out var environment))
                     {
                         return environment;
                     }
                     throw new Exception($"Malformed gitconfig on line {lineNumber} [invalid install environment]: {rawGitconfig}");
                 })
                 .Aggregate((environments, environment) => environments |= environment);
        }

    }

    public class Gitconfig
    {
        public string Path { get; set; } = "";
        public InstallEnvironment Environment { get; set; }
    }
}
