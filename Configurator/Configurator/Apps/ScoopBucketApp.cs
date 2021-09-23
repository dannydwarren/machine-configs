namespace Configurator.Apps
{
    public class ScoopBucketApp : IApp
    {
        public string AppId { get; set; }
        public string? InstallArgs => null;
        public bool PreventUpgrade => false;

        public string InstallScript => $@"scoop bucket add {AppId}";
        public string VerificationScript => @"(scoop bucket list | Select-String {AppId}) -ne $null";
        public string? UpgradeScript => null;

        public AppConfiguration? Configuration => null;
    }
}
