namespace Configurator.Apps
{
    public class ScoopApp : IApp
    {
        public string AppId { get; set; }
        public bool PreventUpgrade { get; set; }

        private string installArgs = "";
        public string InstallArgs
        {
            get => installArgs;
            set => installArgs = string.IsNullOrWhiteSpace(value) ? "" : $" {value}";
        }

        public string InstallScript => $"scoop install {AppId}{InstallArgs}";
        public string VerificationScript => $"(scoop export | Select-String {AppId}) -ne $null";
        public string UpgradeScript => $"scoop update {AppId}";
        public AppConfiguration Configuration { get; set; }
    }
}
