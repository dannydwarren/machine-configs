using Configurator.Git;
using Configurator.PowerShell;
using Configurator.Scoop;
using Configurator.Utilities;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Configurator.UnitTests
{
    public class MachineConfiguratorTests : UnitTestBase<MachineConfigurator>
    {
        [Fact]
        public async Task When_executing_for_specific_install_environment()
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

            GetMock<IArguments>().SetupGet(x => x.Environment).Returns(InstallEnvironment.Personal);
            GetMock<IScoopAppRepository>().Setup(x => x.LoadAsync()).ReturnsAsync(scoopApps);
            GetMock<IGitconfigRepository>().Setup(x => x.LoadAsync()).ReturnsAsync(gitconfigs);

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync());

            It("enables scripts to be executed", () =>
            {
                GetMock<IPowerShellConfiguration>().Verify(x => x.SetExecutionPolicyAsync());
            });

            It("installs apps via scoop matching the install environment", () =>
            {
                GetMock<IScoopInstaller>().Verify(x => x.InstallAsync(scoopApps[0].AppId));
                GetMock<IScoopInstaller>().Verify(x => x.InstallAsync(scoopApps[1].AppId));
            });

            It("configures git", () =>
            {
                GetMock<IGitConfiguration>().Verify(x => x.IncludeGitconfigAsync(gitconfigs[0].Path));
                GetMock<IGitConfiguration>().Verify(x => x.IncludeGitconfigAsync(gitconfigs[1].Path));
            });
        }
    }
}
