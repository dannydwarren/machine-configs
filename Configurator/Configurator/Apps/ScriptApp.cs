namespace Configurator.Apps
{
    public class ScriptApp : IApp
    {
        public string AppId { get; set; }
        public string? InstallArgs => null;
        public bool PreventUpgrade => false;

        public string InstallScript { get; set; }
        public string? VerificationScript { get; set; }
        public string? UpgradeScript { get; set; }

        public AppConfiguration? Configuration { get; set; }
    }
}
