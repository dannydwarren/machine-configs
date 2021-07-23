namespace Configurator.Apps
{
    public class ScoopApp : IApp
    {
        public string AppId { get; set; } = "";
        public string Environments { get; set; } = "";
        public string? DeleteDesktopShortcut { get; set; }

        public string InstallScript => $"scoop install {AppId}";
        public string? VerificationScript => null;
    }
}
