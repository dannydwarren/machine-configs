using System.Collections.Generic;
using System.Text.Json;
using Configurator.Apps;

namespace Configurator
{
    public class ManifestV2
    {
        public List<IApp> Apps { get; set; } = new List<IApp>();
    }

    public class ManifestV2Raw
    {
        public List<JsonElement> Apps { get; set; } = new List<JsonElement>();
    }

    public interface IInstallable
    {
        Installer Installer { get; }
        string Environments { get; }
        JsonElement AppData { get; }
    }

    public enum Installer
    {
        Unknown,
        Script,
        PowerShellAppPackage,
        Winget,
        Scoop,
        ScoopBucket,
        Gitconfig,
        NonPackageApp
    }

    public class Installable : IInstallable
    {
        public Installer Installer { get; set; }
        public string Environments { get; set; } = "";
        public JsonElement AppData { get; set; }
    }
}
