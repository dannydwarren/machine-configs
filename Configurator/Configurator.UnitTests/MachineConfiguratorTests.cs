using Configurator.Git;
using Configurator.PowerShell;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Configurator.Apps;
using Xunit;

namespace Configurator.UnitTests
{
    public class MachineConfiguratorTests : UnitTestBase<MachineConfigurator>
    {
        [Fact]
        public async Task When_executing()
        {
            var apps = new Apps.Apps
            {
                ScoopApps = new List<ScoopApp>
                {
                    new() {AppId = RandomString(), Environments = "Personal"},
                    new() {AppId = RandomString(), Environments = "All"}
                },
                WingetApps = new List<WingetApp>
                {
                    new() {AppId = RandomString(), Environments = "Personal"},
                    new() {AppId = RandomString(), Environments = "Personal"}
                }
            };


            var gitconfigs = new List<Gitconfig>
            {
                new() {Path = RandomString(), Environment = InstallEnvironment.Personal},
                new() {Path = RandomString(), Environment = InstallEnvironment.Personal}
            };

            GetMock<IAppsRepository>().Setup(x => x.LoadAsync()).ReturnsAsync(apps);
            GetMock<IGitconfigRepository>().Setup(x => x.LoadAsync()).ReturnsAsync(gitconfigs);

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync());

            It("enables scripts to be executed",
                () => { GetMock<IPowerShellConfiguration>().Verify(x => x.SetExecutionPolicyAsync()); });

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
    }
}
