namespace Configurator.Apps
{
    public class GitconfigApp : IApp
    {
        public string AppId { get; set; } = "";
        public string? InstallArgs => null;
        public string Environments { get; set; } = "";
        public string InstallScript => @$"git config --global --add include.path {AppId}";
        public string VerificationScript => @$"(git config --get-all --global include.path) -match ""{AppId.Replace(@"\", @"\\")}""";
        public string? UpgradeScript => null;
    }
}
