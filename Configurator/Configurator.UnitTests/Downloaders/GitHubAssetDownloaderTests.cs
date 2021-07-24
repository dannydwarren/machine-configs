using System.Threading.Tasks;
using Configurator.Configuration;
using Configurator.Downloaders;
using Configurator.PowerShell;
using Configurator.Utilities;
using Moq;
using Shouldly;
using Xunit;

namespace Configurator.UnitTests.Downloaders
{
    public class GitHubAssetDownloaderTests : UnitTestBase<GitHubAssetDownloader>
    {
        [Fact]
        public async Task When_downloading()
        {
            var argsJson = RandomString();
            var args = new GitHubAssetDownloaderArgs
            {
                User = RandomString(),
                Repo = RandomString(),
                Extension = RandomString()
            };
            var powerShellResult = new PowerShellResult
            {
                AsString = RandomString()
            };
            var assetInfo = new GitHubAssetInfo
            {
                Filename = RandomString(),
                Url = RandomString()
            };
            var expectedDownloadedFilePath = RandomString();

            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<GitHubAssetDownloaderArgs>(argsJson)).Returns(args);
            GetMock<IPowerShell>().Setup(x => x.ExecuteAsync(Moq.It.Is<string>(y =>
                    y.Contains(args.User) && y.Contains(args.Repo) && y.Contains(args.Extension))))
                .ReturnsAsync(powerShellResult);
            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<GitHubAssetInfo>(powerShellResult.AsString))
                .Returns(assetInfo);
            GetMock<IResourceDownloader>().Setup(x => x.DownloadAsync(assetInfo.Url, assetInfo.Filename))
                .ReturnsAsync(expectedDownloadedFilePath);

            var downloadedFilePath = await BecauseAsync(() => ClassUnderTest.DownloadAsync(argsJson));

            It("returns the downloaded file's path", () =>
            {
                downloadedFilePath.ShouldBe(expectedDownloadedFilePath);
            });
        }
    }
}
