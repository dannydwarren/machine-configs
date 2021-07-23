namespace Configurator.Apps
{
    public class NonPackageApp : IApp
    {
        public string AppId { get; set; } = "";
        public string Environments { get; set; } = "";

        public string InstallScript => $"./{AppId}_install.ps1";
        public string? VerificationScript => null;
    }
}
