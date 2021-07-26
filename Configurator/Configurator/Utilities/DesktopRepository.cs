using System;
using System.Collections.Generic;
using System.IO;
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

        public DesktopRepository(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public List<string> LoadSystemEntries()
        {
            var userDesktopSystemEntries = fileSystem.EnumerateFileSystemEntries(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            var commonDesktopSystemEntries = fileSystem.EnumerateFileSystemEntries(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
            return userDesktopSystemEntries.Union(commonDesktopSystemEntries).ToList();
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
