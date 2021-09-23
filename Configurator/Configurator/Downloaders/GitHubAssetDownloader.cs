using System.Threading.Tasks;
using Configurator.Configuration;
using Configurator.PowerShell;
using Configurator.Utilities;

namespace Configurator.Downloaders
{
    public class GitHubAssetDownloader : IDownloader
    {
        private readonly IJsonSerializer jsonSerializer;
        private readonly IPowerShell powerShell;
        private readonly IResourceDownloader resourceDownloader;

        public GitHubAssetDownloader(IJsonSerializer jsonSerializer,
            IPowerShell powerShell,
            IResourceDownloader resourceDownloader)
        {
            this.jsonSerializer = jsonSerializer;
            this.powerShell = powerShell;
            this.resourceDownloader = resourceDownloader;
        }

        public async Task<string> DownloadAsync(string argsJson)
        {
            var args = jsonSerializer.Deserialize<GitHubAssetDownloaderArgs>(argsJson)!;

            var getAssetInfoScript =
                @$"$asset = (iwr https://api.github.com/repos/{args.User}/{args.Repo}/releases/latest | ConvertFrom-Json).assets | ? {{ $_.name -like '{args.Extension}' }}
$downloadUrl = $asset | select -exp browser_download_url
$fileName = $asset | select -exp name
Write-Output ""{{ `""FileName`"": `""$fileName`"", `""Url`"": `""$downloadUrl`"" }}""";

            var result = await powerShell.ExecuteAsync(getAssetInfoScript);
            var assetInfo = jsonSerializer.Deserialize<GitHubAssetInfo>(result.AsString)!;

            return await resourceDownloader.DownloadAsync(assetInfo.Url, assetInfo.Filename);
        }
    }

    public class GitHubAssetDownloaderArgs
    {
        public string User { get; set; }
        public string Repo { get; set; }
        public string Extension { get; set; }
    }

    public class GitHubAssetInfo
    {
        public string Filename { get; set; }
        public string Url { get; set; }
    }
}
