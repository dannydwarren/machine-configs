using System.Threading.Tasks;

namespace Configurator.Downloaders
{
    public interface IDownloader
    {
        Task<string> DownloadAsync(string argsJson);
    }
}
