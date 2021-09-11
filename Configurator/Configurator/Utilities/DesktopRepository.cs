using System.Collections.Generic;
using System.Linq;

namespace Configurator.Utilities
{
    public interface IDesktopRepository
    {
        List<string> LoadSystemEntries();
        void DeletePaths(List<string> paths);
    }

    public class DesktopRepository : IDesktopRepository
    {
        private readonly IFileSystem fileSystem;
        private readonly ISpecialFolders specialFolders;

        public DesktopRepository(IFileSystem fileSystem, ISpecialFolders specialFolders)
        {
            this.fileSystem = fileSystem;
            this.specialFolders = specialFolders;
        }

        public List<string> LoadSystemEntries()
        {
            return specialFolders.GetDesktopPaths()
                .Select(fileSystem.EnumerateFileSystemEntries)
                .SelectMany(x => x)
                .Distinct()
                .ToList();
        }

        public void DeletePaths(List<string> paths)
        {
            foreach (var path in paths)
            {
                fileSystem.Delete(path);
            }
        }
    }
}
