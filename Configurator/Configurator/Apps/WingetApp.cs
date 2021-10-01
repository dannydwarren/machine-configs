namespace Configurator.Apps
{
    public class WingetApp : IApp
    {
        public string AppId { get; set; }
        public bool PreventUpgrade { get; set; }

        private string installArgs = "";
        public string InstallArgs
        {
            get => installArgs;
            set => installArgs = string.IsNullOrWhiteSpace(value) ? "" : $" {value}";
        }

        public string InstallScript => $"winget install -id {AppId}{InstallArgs}";
        public string VerificationScript => $"(winget list | Select-String {AppId}) -ne $null";
        public string UpgradeScript => $"winget upgrade {AppId}";
        public AppConfiguration Configuration { get; set; }
    }
}
