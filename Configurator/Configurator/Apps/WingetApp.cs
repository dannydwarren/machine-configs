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
            set => installArgs = string.IsNullOrWhiteSpace(value) ? "" : $" --override {value}";
        }

        public string InstallScript => $"winget install --id {AppId} --accept-package-agreements -h -e{InstallArgs}";
        public string VerificationScript => $"(winget list --id {AppId} -e | Select-String {AppId}) -ne $null";
        public string UpgradeScript => $"winget upgrade --id {AppId} --accept-package-agreements -h -e{InstallArgs}";
        public AppConfiguration Configuration { get; set; }
    }
}
