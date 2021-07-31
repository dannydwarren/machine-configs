namespace Configurator.Apps
{
    public class ScoopBucketApp : IApp
    {
        public string AppId { get; set; } = "";
        public string Environments { get; set; } = "";
        public string InstallScript => $@"scoop bucket add {AppId}";
        public string VerificationScript => @"(scoop bucket list | Select-String {AppId}) -ne $null";
    }
}
