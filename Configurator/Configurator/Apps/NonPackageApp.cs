namespace Configurator.Apps
{
    public class NonPackageApp : IApp
    {
        public string AppId { get; set; } = "";
        public string? InstallArgs => null;
        public string Environments { get; set; } = "";

        public string InstallScript => $"./{AppId}_install.ps1";
        public string? VerificationScript => null;
        public string? UpgradeScript => null;
    }
}
