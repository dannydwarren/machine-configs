using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.Downloaders;

namespace Configurator.Installers
{
    public interface IWingetCliInstaller
    {
        Task InstallAsync();
    }

    public class WingetCliInstaller : IWingetCliInstaller
    {
        private readonly IDownloadAppInstaller downloadAppInstaller;

        public static readonly PowerShellAppPackage WingetCliApp = new PowerShellAppPackage
        {
            AppId = "Microsoft.Winget.Source",
            Downloader = nameof(GitHubAssetDownloader),
            DownloaderArgs = JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(@"{
    ""User"": ""microsoft"",
    ""Repo"": ""winget-cli"",
    ""Extension"": ""*.msixbundle""
}"))).RootElement
        };

        public WingetCliInstaller(IDownloadAppInstaller downloadAppInstaller)
        {
            this.downloadAppInstaller = downloadAppInstaller;
        }

        public async Task InstallAsync()
        {
            await downloadAppInstaller.InstallAsync(WingetCliApp);
        }
    }
}
