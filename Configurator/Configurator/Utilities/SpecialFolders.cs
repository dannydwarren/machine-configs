using System;
using System.Collections.Generic;
using System.Linq;

namespace Configurator.Utilities
{
    public interface ISpecialFolders
    {
        List<string> GetDesktopPaths();
    }

    public class SpecialFolders : ISpecialFolders
    {
        public List<string> GetDesktopPaths()
        {
            return new List<string>
                {
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
                }
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }
    }
}
