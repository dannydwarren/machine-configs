using System.Text.Json;

namespace Configurator.Apps
{
    public class PowerShellAppPackage : IDownloadApp
    {
        public string AppId { get; set; } = "";
        public string? InstallArgs => null;
        public bool PreventUpgrade { get; set; } = false;

        public string InstallScript => @"Import-Module appx -UseWindowsPowerShell
Add-AppPackage";
        public string VerificationScript => $@"Import-Module appx -UseWindowsPowerShell
(Get-AppPackage -Name {AppId}) -ne $null";
        public string UpgradeScript => @"Import-Module appx -UseWindowsPowerShell
Add-AppPackage";

        public AppConfiguration? Configuration => null;

        public string Downloader { get; set; } = "";
        public JsonElement DownloaderArgs { get; set; }
    }
}
