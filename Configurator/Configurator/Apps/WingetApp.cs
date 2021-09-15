namespace Configurator.Apps
{
    public class WingetApp : IApp
    {
        public string AppId { get; set; } = "";
        public string Environments { get; set; } = "";

        private string installArgs = "";
        public string InstallArgs
        {
            get => installArgs;
            set => installArgs = string.IsNullOrWhiteSpace(value) ? "" : $" {value}";
        }

        public string InstallScript => $"winget install {AppId}{InstallArgs}";
        public string VerificationScript => $"(winget list | Select-String {AppId}) -ne $null";
        public string UpgradeScript => $"winget upgrade {AppId}";
    }
}
