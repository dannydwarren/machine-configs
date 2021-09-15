namespace Configurator.Apps
{
    public class ScoopBucketApp : IApp
    {
        public string AppId { get; set; } = "";

        private string installArgs = "";
        public string InstallArgs
        {
            get => installArgs;
            set => installArgs = string.IsNullOrWhiteSpace(value) ? "" : $" {value}";
        }

        public string InstallScript => $@"scoop bucket add {AppId}{InstallArgs}";
        public string VerificationScript => @"(scoop bucket list | Select-String {AppId}) -ne $null";
        public string? UpgradeScript => null;
    }
}
