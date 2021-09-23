namespace Configurator.Apps
{
    public class NonPackageApp : IApp
    {
        public string AppId { get; set; }
        public string? InstallArgs => null;
        public bool PreventUpgrade => false;

        public string InstallScript => $"./{AppId}_install.ps1";
        public string? VerificationScript => null;
        public string? UpgradeScript => null;

        public AppConfiguration? Configuration => null;
    }
}
