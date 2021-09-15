using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.Installers;
using Configurator.PowerShell;
using Configurator.Utilities;
using Moq;
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
            var verificationResultPreInstall = new PowerShellResult
            {
                AsString = "False"
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
            GetMock<IPowerShell>().Setup(x => x.ExecuteAsync(app.VerificationScript)).ReturnsAsync(verificationResultPreInstall);

            await BecauseAsync(() => ClassUnderTest.InstallOrUpgradeAsync(app));

            It("logs", () =>
            {
                GetMock<IConsoleLogger>().Verify(x => x.Info($"Installing '{app.AppId}'"));
                GetMock<IConsoleLogger>().Verify(x => x.Result($"Installed '{app.AppId}'"));
            });

            It("runs the install script", () =>
            {
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync(app.VerificationScript), Times.Exactly(2));
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync(app.InstallScript));
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

            GetMock<IDesktopRepository>().Setup(x => x.LoadSystemEntries()).Returns(new List<string>());

            await BecauseAsync(() => ClassUnderTest.InstallOrUpgradeAsync(app));

            It("runs the install script", () =>
            {
                GetMock<IPowerShell>().VerifyNever(x => x.ExecuteAsync(app.VerificationScript!));
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync(app.InstallScript));
            });
        }

        [Fact]
        public async Task When_installing_and_nothing_was_added_to_the_desktop()
        {
            var app = new ScriptApp
            {
                AppId = RandomString(),
                InstallScript = RandomString()
            };

            var desktopSystemEntries = new List<string>
            {
                RandomString(),
            };

            GetMock<IDesktopRepository>().Setup(x => x.LoadSystemEntries()).Returns(desktopSystemEntries);

            await BecauseAsync(() => ClassUnderTest.InstallOrUpgradeAsync(app));

            It("deletes desktop shortcuts", () =>
            {
                GetMock<IDesktopRepository>().VerifyNever(x => x.DeletePaths(IsAny<List<string>>()));
            });
        }

        [Fact]
        public async Task When_upgrading()
        {
            var app = new ScriptApp
            {
                AppId = RandomString(),
                InstallScript = RandomString(),
                VerificationScript = RandomString(),
                UpgradeScript = RandomString()
            };
            var verificationResultPreInstall = new PowerShellResult
            {
                AsString = "True"
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
            GetMock<IPowerShell>().Setup(x => x.ExecuteAsync(app.VerificationScript)).ReturnsAsync(verificationResultPreInstall);

            await BecauseAsync(() => ClassUnderTest.InstallOrUpgradeAsync(app));

            It("runs the upgrade script", () =>
            {
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync(app.VerificationScript), Times.Exactly(2));
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync(app.UpgradeScript));
            });

            It("deletes desktop shortcuts", () =>
            {
                GetMock<IDesktopRepository>().Verify(x => x.DeletePaths(desktopSystemEntriesAddedDuringInstall));
            });
        }

        [Fact]
        public async Task When_already_installed_and_no_upgrade_script_was_provided()
        {
            var app = new ScriptApp
            {
                AppId = RandomString(),
                InstallScript = RandomString(),
                VerificationScript = RandomString(),
                UpgradeScript = null
            };
            var verificationResultPreInstall = new PowerShellResult
            {
                AsString = "True"
            };

            GetMock<IDesktopRepository>().Setup(x => x.LoadSystemEntries()).Returns(new List<string>());
            GetMock<IPowerShell>().Setup(x => x.ExecuteAsync(app.VerificationScript)).ReturnsAsync(verificationResultPreInstall);

            await BecauseAsync(() => ClassUnderTest.InstallOrUpgradeAsync(app));

            It("does not install or upgrade", () =>
            {
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync(app.VerificationScript), Times.Exactly(2));
                GetMock<IPowerShell>().VerifyNever(x => x.ExecuteAsync(app.InstallScript));
                GetMock<IPowerShell>().VerifyNever(x => x.ExecuteAsync(app.UpgradeScript!));
            });
        }
    }
}
