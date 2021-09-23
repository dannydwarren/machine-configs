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
            var mockApp = GetMock<IApp>();
            mockApp.SetupGet(x => x.AppId).Returns(RandomString());
            mockApp.SetupGet(x => x.InstallScript).Returns(RandomString());
            mockApp.SetupGet(x => x.VerificationScript).Returns(RandomString());
            var app = mockApp.Object;

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
            GetMock<IPowerShell>().Setup(x => x.ExecuteAsync(app.VerificationScript!))
                .ReturnsAsync(verificationResultPreInstall);

            await BecauseAsync(() => ClassUnderTest.InstallOrUpgradeAsync(app));

            It("logs", () =>
            {
                GetMock<IConsoleLogger>().Verify(x => x.Info($"Installing '{app.AppId}'"));
                GetMock<IConsoleLogger>().Verify(x => x.Result($"Installed '{app.AppId}'"));
            });

            It("runs the install script", () =>
            {
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync(app.VerificationScript!), Times.Exactly(2));
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
            var mockApp = GetMock<IApp>();
            mockApp.SetupGet(x => x.AppId).Returns(RandomString());
            mockApp.SetupGet(x => x.InstallScript).Returns(RandomString());
            mockApp.SetupGet(x => x.VerificationScript).Returns((string)null!);
            var app = mockApp.Object;

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
            var mockApp = GetMock<IApp>();
            mockApp.SetupGet(x => x.AppId).Returns(RandomString());
            mockApp.SetupGet(x => x.InstallScript).Returns(RandomString());
            var app = mockApp.Object;

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
            var mockApp = GetMock<IApp>();
            mockApp.SetupGet(x => x.AppId).Returns(RandomString());
            mockApp.SetupGet(x => x.InstallScript).Returns(RandomString());
            mockApp.SetupGet(x => x.VerificationScript).Returns(RandomString());
            mockApp.SetupGet(x => x.UpgradeScript).Returns(RandomString());
            var app = mockApp.Object;

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
            GetMock<IPowerShell>().Setup(x => x.ExecuteAsync(app.VerificationScript!))
                .ReturnsAsync(verificationResultPreInstall);

            await BecauseAsync(() => ClassUnderTest.InstallOrUpgradeAsync(app));

            It("runs the upgrade script", () =>
            {
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync(app.VerificationScript!), Times.Exactly(2));
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync(app.UpgradeScript!));
            });

            It("deletes desktop shortcuts", () =>
            {
                GetMock<IDesktopRepository>().Verify(x => x.DeletePaths(desktopSystemEntriesAddedDuringInstall));
            });
        }

        [Fact]
        public async Task When_preventing_upgrade()
        {
            var mockApp = GetMock<IApp>();
            mockApp.SetupGet(x => x.AppId).Returns(RandomString());
            mockApp.SetupGet(x => x.InstallScript).Returns(RandomString());
            mockApp.SetupGet(x => x.VerificationScript).Returns(RandomString());
            mockApp.SetupGet(x => x.UpgradeScript).Returns(RandomString());
            mockApp.SetupGet(x => x.PreventUpgrade).Returns(true);
            var app = mockApp.Object;

            var verificationResultPreInstall = new PowerShellResult
            {
                AsString = "True"
            };

            GetMock<IDesktopRepository>().Setup(x => x.LoadSystemEntries()).Returns(new List<string>());
            GetMock<IPowerShell>().Setup(x => x.ExecuteAsync(app.VerificationScript!))
                .ReturnsAsync(verificationResultPreInstall);

            await BecauseAsync(() => ClassUnderTest.InstallOrUpgradeAsync(app));

            It("does not run the upgrade script", () =>
            {
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync(app.VerificationScript!), Times.Exactly(1));
                GetMock<IPowerShell>().VerifyNever(x => x.ExecuteAsync(app.UpgradeScript!));
            });
        }

        [Fact]
        public async Task When_already_installed_and_no_upgrade_script_was_provided()
        {
            var mockApp = GetMock<IApp>();
            mockApp.SetupGet(x => x.AppId).Returns(RandomString());
            mockApp.SetupGet(x => x.InstallScript).Returns(RandomString());
            mockApp.SetupGet(x => x.VerificationScript).Returns(RandomString());
            mockApp.SetupGet(x => x.UpgradeScript).Returns((string)null!);
            var app = mockApp.Object;
          
            var verificationResultPreInstall = new PowerShellResult
            {
                AsString = "True"
            };

            GetMock<IDesktopRepository>().Setup(x => x.LoadSystemEntries()).Returns(new List<string>());
            GetMock<IPowerShell>().Setup(x => x.ExecuteAsync(app.VerificationScript!))
                .ReturnsAsync(verificationResultPreInstall);

            await BecauseAsync(() => ClassUnderTest.InstallOrUpgradeAsync(app));

            It("does not install or upgrade", () =>
            {
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync(app.VerificationScript!), Times.Exactly(1));
                GetMock<IPowerShell>().VerifyNever(x => x.ExecuteAsync(app.InstallScript));
                GetMock<IPowerShell>().VerifyNever(x => x.ExecuteAsync(app.UpgradeScript!));
            });
        }
    }
}
