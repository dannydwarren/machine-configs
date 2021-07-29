using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.Installers;
using Configurator.PowerShell;
using Configurator.Utilities;
using Xunit;

namespace Configurator.UnitTests.Installers
{
    public class AppInstallerTests : UnitTestBase<AppInstaller>
    {
        [Fact]
        public async Task When_installing()
        {
            var app = new ScriptApp
            {
                AppId = RandomString(),
                InstallScript = RandomString(),
                VerificationScript = RandomString()
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

            It("logs", () =>
            {
                GetMock<IConsoleLogger>().Verify(x => x.Info($"Installing '{app.AppId}'"));
                GetMock<IConsoleLogger>().Verify(x => x.Result($"Installed '{app.AppId}'"));
            });

            It("runs the install script", () =>
            {
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync(app.InstallScript, app.VerificationScript));
            });

            It("deletes desktop shortcuts", () =>
            {
                GetMock<IDesktopRepository>().Verify(x => x.DeletePaths(desktopSystemEntriesAddedDuringInstall));
            });
        }

        [Fact]
        public async Task When_installing_and_no_verification_script_was_provided()
        {
            var app = new ScriptApp
            {
                AppId = RandomString(),
                InstallScript = RandomString(),
                VerificationScript = null
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

            It("logs", () =>
            {
                GetMock<IConsoleLogger>().Verify(x => x.Info($"Installing '{app.AppId}'"));
                GetMock<IConsoleLogger>().Verify(x => x.Result($"Installed '{app.AppId}'"));
            });

            It("runs the install script", () =>
            {
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync(app.InstallScript));
            });

            It("deletes desktop shortcuts", () =>
            {
                GetMock<IDesktopRepository>().Verify(x => x.DeletePaths(desktopSystemEntriesAddedDuringInstall));
            });
        }

        [Fact]
        public async Task When_installing_and_nothing_was_added_to_the_desktop()
        {
            var app = new WingetApp
            {
                AppId = RandomString()
            };

            var desktopSystemEntries = new List<string>
            {
                RandomString(),
            };

            GetMock<IDesktopRepository>().Setup(x => x.LoadSystemEntries()).Returns(desktopSystemEntries);

            await BecauseAsync(() => ClassUnderTest.InstallAsync(app));

            It("deletes desktop shortcuts", () =>
            {
                GetMock<IDesktopRepository>().VerifyNever(x => x.DeletePaths(IsAny<List<string>>()));
            });
        }
    }
}
