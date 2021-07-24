using System.Collections.Generic;

namespace Configurator.Apps
{
    public class Apps
    {
        public List<WingetApp> WingetApps { get; set; } = new();
        public List<ScoopApp> ScoopApps { get; set; } = new();
        public List<NonPackageApp> NonPackageApps { get; set; } = new();
        public List<PowerShellAppPackage> PowerShellAppPackages { get; set; } = new();
    }
}
