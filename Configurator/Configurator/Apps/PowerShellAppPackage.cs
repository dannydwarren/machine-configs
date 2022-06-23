using System.Text.Json;

namespace Configurator.Apps
{
    public class PowerShellAppPackage : IDownloadApp
    {
        public string AppId { get; set; }
        public string? InstallArgs => null;
        public bool PreventUpgrade { get; set; }

        public string InstallScript => $@"Import-Module appx -UseWindowsPowerShell
Add-AppPackage {DownloadedFilePath}";
        public string VerificationScript => $@"Import-Module appx -UseWindowsPowerShell
(Get-AppPackage -Name {AppId}) -ne $null";
        public string UpgradeScript => InstallScript;

        public AppConfiguration? Configuration => null;

        public string Downloader { get; set; }
        public string DownloadedFilePath { get; set; }
        public JsonElement DownloaderArgs { get; set; }
    }
}
