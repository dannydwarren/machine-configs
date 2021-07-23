namespace Configurator.Apps
{
    public interface IApp
    {
        string AppId { get; set; }
        string Environments { get; set; }
        string InstallScript { get; }
        string? VerificationScript { get; }
    }
}
