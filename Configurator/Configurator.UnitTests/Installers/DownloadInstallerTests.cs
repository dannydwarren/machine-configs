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
    public class DownloadInstallerTests : UnitTestBase<DownloadInstaller>
    {
        [Fact]
        public async Task When_installing()
        {
            var appMock = GetMock<IDownloadApp>();
            appMock.SetupGet(x => x.AppId).Returns(RandomString());
            appMock.SetupGet(x => x.InstallScript).Returns(RandomString());
            appMock.SetupGet(x => x.VerificationScript).Returns(RandomString());
            appMock.SetupGet(x => x.Downloader).Returns(RandomString());
            appMock.SetupGet(x => x.DownloaderArgs).Returns(RandomString());
            var app = appMock.Object;

            var downloadedFilePath = RandomString();

            var downloaderMock = GetMock<IDownloader>();
            downloaderMock.Setup(x => x.DownloadAsync(app.DownloaderArgs)).ReturnsAsync(downloadedFilePath);
            var downloader = downloaderMock.Object;

            GetMock<IDownloaderFactory>().Setup(x => x.GetDownloader(app.Downloader)).Returns(downloader);

            await BecauseAsync(() => ClassUnderTest.InstallAsync(app));

            It("logs", () =>
            {
                GetMock<IConsoleLogger>().Verify(x => x.Info($"Installing '{app.AppId}'"));
                GetMock<IConsoleLogger>().Verify(x => x.Result($"Installed '{app.AppId}'"));
            });

            It("installs and verifies", () =>
            {
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync($"{app.InstallScript} {downloadedFilePath}", app.VerificationScript!));
            });
        }

        [Fact]
        public async Task When_installing_with_no_verification_script()
        {
            var appMock = GetMock<IDownloadApp>();
            appMock.SetupGet(x => x.AppId).Returns(RandomString());
            appMock.SetupGet(x => x.InstallScript).Returns(RandomString());
            appMock.SetupGet(x => x.VerificationScript).Returns((string)null!);
            appMock.SetupGet(x => x.Downloader).Returns(RandomString());
            appMock.SetupGet(x => x.DownloaderArgs).Returns(RandomString());
            var app = appMock.Object;

            var downloadedFilePath = RandomString();

            var downloaderMock = GetMock<IDownloader>();
            downloaderMock.Setup(x => x.DownloadAsync(app.DownloaderArgs)).ReturnsAsync(downloadedFilePath);
            var downloader = downloaderMock.Object;

            GetMock<IDownloaderFactory>().Setup(x => x.GetDownloader(app.Downloader)).Returns(downloader);

            await BecauseAsync(() => ClassUnderTest.InstallAsync(app));

            It("logs", () =>
            {
                GetMock<IConsoleLogger>().Verify(x => x.Info($"Installing '{app.AppId}'"));
                GetMock<IConsoleLogger>().Verify(x => x.Result($"Installed '{app.AppId}'"));
            });

            It("installs", () =>
            {
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync($"{app.InstallScript} {downloadedFilePath}"));
            });
        }
    }
}
