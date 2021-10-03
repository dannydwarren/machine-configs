using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.Downloaders;
using Configurator.PowerShell;

namespace Configurator.Installers
{
    public interface IWingetCliInstaller
    {
        Task InstallAsync();
    }

    public class WingetCliInstaller : IWingetCliInstaller
    {
        private readonly IDownloadAppInstaller downloadAppInstaller;
        private readonly IPowerShell powerShell;

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

        public WingetCliInstaller(IDownloadAppInstaller downloadAppInstaller, IPowerShell powerShell)
        {
            this.downloadAppInstaller = downloadAppInstaller;
            this.powerShell = powerShell;
        }

        public async Task InstallAsync()
        {
            await downloadAppInstaller.InstallAsync(WingetCliApp);
            await powerShell.ExecuteAsync("winget list winget --accept-source-agreements");
        }
    }
}
