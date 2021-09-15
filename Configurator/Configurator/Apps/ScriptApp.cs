namespace Configurator.Apps
{
    public class ScriptApp : IApp
    {
        public string AppId { get; set; } = "";
        public string Environments { get; set; } = "";
        public string InstallScript { get; set; } = "";
        public string? VerificationScript { get; set; }
        public string? UpgradeScript { get; set; }
    }
}
