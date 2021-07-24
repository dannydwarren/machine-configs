using System.Text.Json;

namespace Configurator.Apps
{
    public interface IApp
    {
        string AppId { get; }
        string Environments { get; }
        string InstallScript { get; }
        string? VerificationScript { get; }
    }

    public interface IDownloadApp : IApp
    {
        string Downloader { get; }
        JsonElement DownloaderArgs { get; }
    }
}