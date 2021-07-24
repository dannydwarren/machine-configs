using System;
using System.IO;
using System.Threading.Tasks;
using Configurator.Utilities;
using Emmersion.Http;
using Moq;
using Shouldly;
using Xunit;

namespace Configurator.UnitTests.Utilities
{
    public class ResourceDownloaderTests : UnitTestBase<ResourceDownloader>
    {
        [Fact]
        public async Task When_downloading()
        {
            var downloadsDir = RandomString();
            var fileName = RandomString();
            var fileUrl = RandomString();
            var httpResponse = new HttpStreamResponse(200, new HttpHeaders(), new MemoryStream());
            IHttpRequest? capturedHttpRequest = null;
            string? capturedPath = null;
            var expectedFilePath = $"{downloadsDir}\\{fileName}";

            GetMock<IArguments>().SetupGet(x => x.DownloadsDir).Returns(downloadsDir);

            GetMock<IHttpClient>().Setup(x => x.ExecuteAsStreamAsync(IsAny<IHttpRequest>()))
                .Callback<IHttpRequest>(httpRequest => capturedHttpRequest = httpRequest)
                .ReturnsAsync(httpResponse);

            GetMock<IFileSystem>().Setup(x => x.WriteStreamAsync(IsAny<string>(), httpResponse.Stream))
                .Callback<string, Stream>((path, _) => capturedPath = path);

            var downloadedFilePath = await BecauseAsync(() => ClassUnderTest.DownloadAsync(fileUrl, fileName));

            It("downloads the required file", () =>
            {
                capturedHttpRequest.ShouldNotBeNull().ShouldSatisfyAllConditions(x =>
                {
                    x.Url.ShouldBe(fileUrl);
                    x.Method.ShouldBe(HttpMethod.GET);
                });

                capturedPath.ShouldBe(expectedFilePath);
            });

            It("returns the path to the downloaded file", () =>
            {
                downloadedFilePath.ShouldBe(expectedFilePath);
            });
        }

        [Fact]
        public async Task When_downloading_fails()
        {
            var fileName = RandomString();
            var fileUrl = RandomString();
            var httpResponse = new HttpStreamResponse(500, new HttpHeaders(), new MemoryStream());

            GetMock<IHttpClient>().Setup(x => x.ExecuteAsStreamAsync(IsAny<IHttpRequest>()))
                .ReturnsAsync(httpResponse);

            var exception = await BecauseThrowsAsync<Exception>(() => ClassUnderTest.DownloadAsync(fileUrl, fileName));

            It("throws an informative exception", () =>
            {
                exception.ShouldNotBeNull()
                    .Message.ShouldBe($"Failed with status code {httpResponse.StatusCode} to download {fileName}");
            });
        }
    }
}
