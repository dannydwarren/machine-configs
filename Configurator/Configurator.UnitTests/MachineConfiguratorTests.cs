using Configurator.Git;
using Configurator.PowerShell;
using Configurator.Scoop;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Configurator.Winget;
using Xunit;

namespace Configurator.UnitTests
{
    public class MachineConfiguratorTests : UnitTestBase<MachineConfigurator>
    {
        [Fact]
        public async Task When_executing()
        {
            var scoopApps = new List<ScoopApp>
            {
                new ScoopApp{ AppId = RandomString(), Environment = InstallEnvironment.Personal },
                new ScoopApp{ AppId = RandomString(), Environment = InstallEnvironment.All }
            };

            var gitconfigs = new List<Gitconfig>
            {
                new Gitconfig{ Path = RandomString(), Environment = InstallEnvironment.Personal},
                new Gitconfig{ Path = RandomString(), Environment = InstallEnvironment.Personal}
            };

            var wingetApps = new List<WingetApp>
            {
                new WingetApp{AppId = RandomString(), Environment = InstallEnvironment.Personal},
                new WingetApp{AppId = RandomString(), Environment = InstallEnvironment.Personal}
            };

            GetMock<IScoopAppRepository>().Setup(x => x.LoadAsync()).ReturnsAsync(scoopApps);
            GetMock<IGitconfigRepository>().Setup(x => x.LoadAsync()).ReturnsAsync(gitconfigs);
            GetMock<IWingetAppRepository>().Setup(x => x.LoadAsync()).ReturnsAsync(wingetApps);

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync());

            It("enables scripts to be executed", () =>
            {
                GetMock<IPowerShellConfiguration>().Verify(x => x.SetExecutionPolicyAsync());
            });

            It("installs apps via scoop", () =>
            {
                GetMock<IScoopInstaller>().Verify(x => x.InstallAsync(scoopApps[0].AppId));
                GetMock<IScoopInstaller>().Verify(x => x.InstallAsync(scoopApps[1].AppId));
            });

            It("configures git", () =>
            {
                GetMock<IGitConfiguration>().Verify(x => x.IncludeGitconfigAsync(gitconfigs[0].Path));
                GetMock<IGitConfiguration>().Verify(x => x.IncludeGitconfigAsync(gitconfigs[1].Path));
            });

            It("installs apps via winget", () =>
            {
                GetMock<IWingetAppInstaller>().Verify(x => x.InstallAsync(wingetApps[0]));
                GetMock<IWingetAppInstaller>().Verify(x => x.InstallAsync(wingetApps[1]));
            });
        }
    }
}
