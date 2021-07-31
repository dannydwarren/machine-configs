using System.Collections.Generic;
using Configurator.Apps;

namespace Configurator
{
    public class Manifest
    {
        public List<WingetApp> WingetApps { get; set; } = new List<WingetApp>();
        public List<ScoopBucketApp> ScoopBuckets { get; set; } = new List<ScoopBucketApp>();
        public List<ScoopApp> ScoopApps { get; set; } = new List<ScoopApp>();
        public List<NonPackageApp> NonPackageApps { get; set; } = new List<NonPackageApp>();
        public List<PowerShellAppPackage> PowerShellAppPackages { get; set; } = new List<PowerShellAppPackage>();
        public List<GitconfigApp> Gitconfigs { get; set; } = new List<GitconfigApp>();
    }
}
