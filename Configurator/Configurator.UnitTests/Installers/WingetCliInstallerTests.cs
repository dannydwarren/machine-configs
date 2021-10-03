using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.Installers;
using Configurator.PowerShell;
using Shouldly;
using Xunit;

namespace Configurator.UnitTests.Installers
{
    public class WingetCliInstallerTests : UnitTestBase<WingetCliInstaller>
    {
        [Fact]
        public async Task When_installing_winget_cli()
        {
            IDownloadApp capturedDownloadApp = null!;
            GetMock<IDownloadAppInstaller>().Setup(x => x.InstallAsync(IsAny<IDownloadApp>()))
                .Callback<IDownloadApp>(downloadApp => capturedDownloadApp = downloadApp);

            await BecauseAsync(() => ClassUnderTest.InstallAsync());

            It("installs", () =>
            {
                capturedDownloadApp.ShouldBe(WingetCliInstaller.WingetCliApp);
            });

            It("accepts all source agreements", () =>
            {
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync("winget list winget --accept-source-agreements"));
            });
        }
    }
}
