using System;
using Configurator.Git;
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
            var apps = new Configurator.Apps.Apps
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
                }
            };

            var gitconfigs = new List<Gitconfig>
            {
                new Gitconfig {Path = RandomString(), Environment = InstallEnvironment.Personal},
                new Gitconfig {Path = RandomString(), Environment = InstallEnvironment.Personal}
            };

            GetMock<IAppsRepository>().Setup(x => x.LoadAsync()).ReturnsAsync(apps);
            GetMock<IGitconfigRepository>().Setup(x => x.LoadAsync()).ReturnsAsync(gitconfigs);

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync());

            It("enables scripts to be executed",
                () => { GetMock<IPowerShellConfiguration>().Verify(x => x.SetExecutionPolicyAsync()); });

            It("installs PowerShell app packages", () =>
            {
                GetMock<IDownloadInstaller>().Verify(x => x.InstallAsync(apps.PowerShellAppPackages[0]));
                GetMock<IDownloadInstaller>().Verify(x => x.InstallAsync(apps.PowerShellAppPackages[1]));
            });

            It("installs apps via scoop", () =>
            {
                GetMock<IAppInstaller>().Verify(x => x.InstallAsync(apps.ScoopApps[0]));
                GetMock<IAppInstaller>().Verify(x => x.InstallAsync(apps.ScoopApps[1]));
            });

            It("configures git", () =>
            {
                GetMock<IGitConfiguration>().Verify(x => x.IncludeGitconfigAsync(gitconfigs[0].Path));
                GetMock<IGitConfiguration>().Verify(x => x.IncludeGitconfigAsync(gitconfigs[1].Path));
            });

            It("installs apps via winget", () =>
            {
                GetMock<IAppInstaller>().Verify(x => x.InstallAsync(apps.WingetApps[0]));
                GetMock<IAppInstaller>().Verify(x => x.InstallAsync(apps.WingetApps[1]));
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
