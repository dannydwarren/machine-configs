using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Configurator.Utilities
{
    public interface IFileSystem
    {
        Task<List<string>> ReadAllLinesAsync(string path);
    }

    public class FileSystem : IFileSystem
    {
        public async Task<List<string>> ReadAllLinesAsync(string path)
        {
            return (await File.ReadAllLinesAsync(path)).ToList();
        }
    }
}
