namespace Configurator.Apps
{
    public class WingetApp : IApp
    {
        public string AppId { get; set; } = "";
        public string Environments { get; set; } = "";

        public string InstallScript => $"winget install {AppId}";
        public string? VerificationScript => null;
    }
}
