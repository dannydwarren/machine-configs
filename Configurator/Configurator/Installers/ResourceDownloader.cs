using System;
using Emmersion.Http;
using System.Threading.Tasks;
using Configurator.Utilities;

namespace Configurator.Installers
{
    public class ResourceDownloader
    {
        private readonly IArguments arguments;
        private readonly IHttpClient httpClient;
        private readonly IFileSystem fileSystem;

        public ResourceDownloader(IArguments arguments,
            IHttpClient httpClient,
            IFileSystem fileSystem)
        {
            this.arguments = arguments;
            this.httpClient = httpClient;
            this.fileSystem = fileSystem;
        }

        public async Task<string> ExecuteAsync(string fileUrl, string fileName)
        {
            var httpRequest = new HttpRequest
            {
                Url = fileUrl,
                Method = HttpMethod.GET
            };

            var response = await httpClient.ExecuteAsStreamAsync(httpRequest);

            if (response.StatusCode != 200)
            {
                throw new Exception($"Failed with status code {response.StatusCode} to download {fileName}");
            }

            var filePath = $"{arguments.DownloadsDir}\\{fileName}";
            await fileSystem.WriteStreamAsync(filePath, response.Stream);

            return filePath;
        }
    }
}
