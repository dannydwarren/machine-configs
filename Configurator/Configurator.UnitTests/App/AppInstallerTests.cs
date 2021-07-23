using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.PowerShell;
using Configurator.Utilities;
using Xunit;

namespace Configurator.UnitTests.App
{
    public class AppInstallerTests : UnitTestBase<AppInstaller>
    {
        [Fact]
        public async Task When_installing()
        {
            var app = new WingetApp
            {
                AppId = RandomString()
            };

            await BecauseAsync(() => ClassUnderTest.InstallAsync(app));

            It("runs the install script", () =>
            {
                GetMock<IConsoleLogger>().Verify(x => x.Info($"Installing '{app.AppId}'"));
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync(app.InstallScript));
                GetMock<IConsoleLogger>().Verify(x => x.Result($"Installed '{app.AppId}'"));
            });
        }
    }
}
