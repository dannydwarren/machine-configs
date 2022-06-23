using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Configurator.Configuration;
using Configurator.Utilities;
using Emmersion.Http;

namespace Configurator.Downloaders
{
    public class VisualStudioMarketplaceDownloader : IDownloader
    {
        private readonly IJsonSerializer jsonSerializer;
        private readonly IHttpClient httpClient;
        private readonly IResourceDownloader resourceDownloader;

        public VisualStudioMarketplaceDownloader(IJsonSerializer jsonSerializer,
            IHttpClient httpClient,
            IResourceDownloader resourceDownloader)
        {
            this.jsonSerializer = jsonSerializer;
            this.httpClient = httpClient;
            this.resourceDownloader = resourceDownloader;
        }

        public async Task<string> DownloadAsync(string argsJson)
        {
            var args = jsonSerializer.Deserialize<VisualStudioMarketplaceDownloaderArgs>(argsJson);
            var extensionPageHtml = await GetExtensionPageHtml(args);
            var extensionDownloadUrl = BuildExtensionDownloadUrl(args, extensionPageHtml);
            var extensionFileName = $"{args.Publisher}.{args.ExtensionName}.vsix";
            return await resourceDownloader.DownloadAsync(extensionDownloadUrl, extensionFileName);
        }

        private async Task<string> GetExtensionPageHtml(VisualStudioMarketplaceDownloaderArgs args)
        {
            var httpRequest = new HttpRequest
            {
                Url = $"https://marketplace.visualstudio.com/items?itemName={args.Publisher}.{args.ExtensionName}"
            };
            var httpResponse = await httpClient.ExecuteAsync(httpRequest);
            var extensionPageHtml = httpResponse.Body;
            return extensionPageHtml;
        }

        private static string BuildExtensionDownloadUrl(VisualStudioMarketplaceDownloaderArgs args, string extensionPageHtml)
        {
            var versionedRelativeUrlRegex = new Regex(
                $"/_apis/public/gallery/publishers/{args.Publisher}/vsextensions/{args.ExtensionName}/(\\d+\\.?)+/vspackage");
            var versionedRelativeUrl = versionedRelativeUrlRegex.Match(extensionPageHtml).Value;
            var extensionDownloadUrl = $"https://marketplace.visualstudio.com{versionedRelativeUrl}";
            return extensionDownloadUrl;
        }
    }

    public class VisualStudioMarketplaceDownloaderArgs
    {
        public string Publisher { get; set; }
        public string ExtensionName { get; set; }
    }
}
