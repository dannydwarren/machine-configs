using System.Threading.Tasks;
using Configurator.PowerShell;
using Configurator.Utilities;
using Configurator.Winget;
using Xunit;

namespace Configurator.UnitTests.Winget
{
    public class WingetAppInstallerTests : UnitTestBase<WingetAppInstaller>
    {
        [Fact]
        public async Task When_installing()
        {
            var app = new WingetApp
            {
                AppId = RandomString()
            };

            await BecauseAsync(() => ClassUnderTest.InstallAsync(app));

            It("invokes winget via powershell", () =>
            {
                GetMock<IConsoleLogger>().Verify(x => x.Info($"Installing '{app.AppId}'"));
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync($"winget install {app.AppId}"));
                GetMock<IConsoleLogger>().Verify(x => x.Result($"Installed '{app.AppId}'"));
            });
        }
    }
}
