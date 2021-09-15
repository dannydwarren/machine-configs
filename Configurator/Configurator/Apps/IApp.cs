using System.Text.Json;

namespace Configurator.Apps
{
    public interface IApp
    {
        string AppId { get; }
        string? InstallArgs { get; }
        string Environments { get; }
        string InstallScript { get; }
        string? VerificationScript { get; }
        string? UpgradeScript { get; }
    }

    public interface IDownloadApp : IApp
    {
        string Downloader { get; }
        JsonElement DownloaderArgs { get; }
    }
}
