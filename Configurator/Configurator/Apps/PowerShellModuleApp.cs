namespace Configurator.Apps
{
    public class PowerShellModuleApp : IApp
    {
        public string AppId { get; set; }
        public bool PreventUpgrade { get; set; }

        private string installArgs = "";
        public string InstallArgs
        {
            get => installArgs;
            set => installArgs = string.IsNullOrWhiteSpace(value) ? "" : $" {value}";
        }

        public string InstallScript => $"Install-Module -Name {AppId}{InstallArgs}";
        public string VerificationScript => $"(Get-Module -ListAvailable {AppId}) -ne $null";
        public string UpgradeScript => $"Update-Module -Name {AppId}{InstallArgs}";
        public AppConfiguration Configuration { get; set; }
    }
}
