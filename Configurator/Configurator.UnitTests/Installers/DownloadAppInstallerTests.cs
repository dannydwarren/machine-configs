using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.Downloaders;
using Configurator.Installers;
using Configurator.PowerShell;
using Configurator.Utilities;
using Moq;
using Xunit;

namespace Configurator.UnitTests.Installers
{
    public class DownloadAppInstallerTests : UnitTestBase<DownloadAppInstaller>
    {
        [Fact]
        public async Task When_installing()
        {
            var buffer = Encoding.UTF8.GetBytes("{ \"prop1\": 1 }");
            var memoryStream = new MemoryStream(buffer);
            var downloaderArgsDoc = await JsonDocument.ParseAsync(memoryStream);

            var mockApp = GetMock<IDownloadApp>();
            mockApp.SetupGet(x => x.AppId).Returns(RandomString());
            mockApp.SetupGet(x => x.InstallScript).Returns(RandomString());
            mockApp.SetupGet(x => x.VerificationScript).Returns(RandomString());
            mockApp.SetupGet(x => x.Downloader).Returns(RandomString());
            mockApp.SetupGet(x => x.DownloaderArgs).Returns(downloaderArgsDoc.RootElement);
            var app = mockApp.Object;

            var downloadedFilePath = RandomString();

            var downloaderMock = GetMock<IDownloader>();
            downloaderMock.Setup(x => x.DownloadAsync(app.DownloaderArgs.ToString()!)).ReturnsAsync(downloadedFilePath);
            var downloader = downloaderMock.Object;

            GetMock<IDownloaderFactory>().Setup(x => x.GetDownloader(app.Downloader)).Returns(downloader);

            await BecauseAsync(() => ClassUnderTest.InstallAsync(app));

            It("logs", () =>
            {
                GetMock<IConsoleLogger>().Verify(x => x.Info($"Installing '{app.AppId}'"));
                GetMock<IConsoleLogger>().Verify(x => x.Result($"Installed '{app.AppId}'"));
            });

            It($"sets {nameof(IDownloadApp)}.{nameof(IDownloadApp.DownloadedFilePath)}", () =>
            {
                mockApp.VerifySet(x => x.DownloadedFilePath = downloadedFilePath);
            });

            It("installs and verifies", () =>
            {
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync($"{app.InstallScript}", app.VerificationScript!));
            });
        }

        [Fact]
        public async Task When_installing_with_no_verification_script()
        {
            var buffer = Encoding.UTF8.GetBytes("{ \"prop1\": 1 }");
            var memoryStream = new MemoryStream(buffer);
            var downloaderArgsDoc = await JsonDocument.ParseAsync(memoryStream);

            var mockApp = GetMock<IDownloadApp>();
            mockApp.SetupGet(x => x.AppId).Returns(RandomString());
            mockApp.SetupGet(x => x.InstallScript).Returns(RandomString());
            mockApp.SetupGet(x => x.VerificationScript).Returns((string)null!);
            mockApp.SetupGet(x => x.Downloader).Returns(RandomString());
            mockApp.SetupGet(x => x.DownloaderArgs).Returns(downloaderArgsDoc.RootElement);
            var app = mockApp.Object;

            var downloadedFilePath = RandomString();

            var downloaderMock = GetMock<IDownloader>();
            downloaderMock.Setup(x => x.DownloadAsync(app.DownloaderArgs.ToString()!)).ReturnsAsync(downloadedFilePath);
            var downloader = downloaderMock.Object;

            GetMock<IDownloaderFactory>().Setup(x => x.GetDownloader(app.Downloader)).Returns(downloader);

            await BecauseAsync(() => ClassUnderTest.InstallAsync(app));

            It("installs", () =>
            {
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync($"{app.InstallScript}"));
            });
        }
    }
}
