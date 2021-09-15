namespace Configurator.Apps
{
    public class WingetApp : IApp
    {
        public string AppId { get; set; } = "";
        public string Environments { get; set; } = "";

        public string InstallScript => $"winget install {AppId}";
        public string VerificationScript => $"(winget list | Select-String {AppId}) -ne $null";
        public string UpgradeScript => $"winget upgrade {AppId}";
    }
}
