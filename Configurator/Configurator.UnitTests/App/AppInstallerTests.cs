using System.Collections.Generic;
using System.Linq;
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

            var desktopSystemEntriesPreInstall = new List<string>
            {
                RandomString(),
            };

            var desktopSystemEntriesAddedDuringInstall = new List<string>
            {
                RandomString(),
                RandomString(),
            };

            var desktopSystemEntriesPostInstall =
                desktopSystemEntriesPreInstall.Union(desktopSystemEntriesAddedDuringInstall).ToList();

            bool isPreInstall = true;
            GetMock<IDesktopRepository>().Setup(x => x.LoadSystemEntries()).Returns(() =>
            {
                if (isPreInstall)
                {
                    isPreInstall = false;
                    return desktopSystemEntriesPreInstall;
                }
                return desktopSystemEntriesPostInstall;
            });

            await BecauseAsync(() => ClassUnderTest.InstallAsync(app));

            It("runs the install script", () =>
            {
                GetMock<IConsoleLogger>().Verify(x => x.Info($"Installing '{app.AppId}'"));
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync(app.InstallScript));
                GetMock<IConsoleLogger>().Verify(x => x.Result($"Installed '{app.AppId}'"));
            });

            It("deletes desktop shortcuts", () =>
            {
                GetMock<IDesktopRepository>().Verify(x => x.DeletePaths(desktopSystemEntriesAddedDuringInstall));
            });
        }
    }
}
