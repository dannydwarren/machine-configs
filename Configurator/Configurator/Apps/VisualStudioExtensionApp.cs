using System.Text.Json;
using Configurator.Downloaders;

namespace Configurator.Apps
{
    public class VisualStudioExtensionApp : IDownloadApp
    {
        public string AppId { get; set; }
        public string? InstallArgs => null;
        public bool PreventUpgrade { get; set; }

        public string InstallScript => @$"$vsixInstaller = . ""${{env:ProgramFiles(x86)}}\Microsoft Visual Studio\Installer\vswhere.exe"" -property productPath | Split-Path | % {{ ""$_\VSIXInstaller.exe"" }}
$installArgs = ""/quiet"", ""/admin"", ""{DownloadedFilePath}""
Start-Process $vsi0xInstaller $installArgs -Wait";
        public string? VerificationScript => null;
        public string UpgradeScript => InstallScript;

        public AppConfiguration? Configuration => null;

        public string Downloader => nameof(VisualStudioMarketplaceDownloader);
        public string DownloadedFilePath { get; set; }
        public JsonElement DownloaderArgs { get; set; }
    }
}
