using System;
using Configurator.PowerShell;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Configurator.Apps;
using Configurator.Installers;
using Configurator.Utilities;
using Xunit;

namespace Configurator.UnitTests
{
    public class MachineConfiguratorTests : UnitTestBase<MachineConfigurator>
    {
        [Fact]
        public async Task When_executing()
        {
            var manifest = new Manifest
            {
                PowerShellAppPackages = new List<PowerShellAppPackage>
                {
                    new PowerShellAppPackage {AppId = RandomString(), Environments = "Personal"},
                    new PowerShellAppPackage {AppId = RandomString(), Environments = "All"}
                },
                ScoopApps = new List<ScoopApp>
                {
                    new ScoopApp {AppId = RandomString(), Environments = "Personal"},
                    new ScoopApp {AppId = RandomString(), Environments = "All"}
                },
                WingetApps = new List<WingetApp>
                {
                    new WingetApp {AppId = RandomString(), Environments = "Personal"},
                    new WingetApp {AppId = RandomString(), Environments = "Personal"}
                },
                Gitconfigs = new List<GitconfigApp>
                {
                    new GitconfigApp {AppId = RandomString(), Environments = "Personal"},
                    new GitconfigApp {AppId = RandomString(), Environments = "Personal"}
                }
            };

            GetMock<IManifestRepository>().Setup(x => x.LoadAsync()).ReturnsAsync(manifest);

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync());

            It("enables scripts to be executed",
                () => { GetMock<IPowerShellConfiguration>().Verify(x => x.SetExecutionPolicyAsync()); });

            It("installs PowerShell app packages", () =>
            {
                GetMock<IDownloadInstaller>().Verify(x => x.InstallAsync(manifest.PowerShellAppPackages[0]));
                GetMock<IDownloadInstaller>().Verify(x => x.InstallAsync(manifest.PowerShellAppPackages[1]));
            });

            It("installs apps via scoop", () =>
            {
                GetMock<IAppInstaller>().Verify(x => x.InstallAsync(manifest.ScoopApps[0]));
                GetMock<IAppInstaller>().Verify(x => x.InstallAsync(manifest.ScoopApps[1]));
            });

            It("installs gitconfigs", () =>
            {
                GetMock<IAppInstaller>().Verify(x => x.InstallAsync(manifest.Gitconfigs[0]));
                GetMock<IAppInstaller>().Verify(x => x.InstallAsync(manifest.Gitconfigs[1]));
            });

            It("installs apps via winget", () =>
            {
                GetMock<IAppInstaller>().Verify(x => x.InstallAsync(manifest.WingetApps[0]));
                GetMock<IAppInstaller>().Verify(x => x.InstallAsync(manifest.WingetApps[1]));
            });
        }

        [Fact]
        public async Task When_an_exception_occurs()
        {
            var exception = new Exception(RandomString());

            GetMock<IPowerShellConfiguration>().Setup(x => x.SetExecutionPolicyAsync()).Throws(exception);

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync());

            It("logs the exception as an error", () =>
            {
                GetMock<IConsoleLogger>().Setup(x => x.Error(exception.ToString()));
            });
        }
    }
}
