using Configurator.Installers;
using Configurator.Lists;
using Configurator.PowerShell;
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
                new ScoopApp{ AppId = RandomString(), Environment = InstallEnvironment.Work },
                new ScoopApp{ AppId = RandomString(), Environment = InstallEnvironment.All }
            };

            GetMock<IArguments>().SetupGet(x => x.Environment).Returns(InstallEnvironment.Personal);
            GetMock<IScoopList>().Setup(x => x.LoadAsync()).ReturnsAsync(scoopApps);

            await BecauseAsync(() => ClassUnderTest.ExecuteAsync());

            It("enables scripts to be executed", () =>
            {
                GetMock<IPowerShellConfiguration>().Verify(x => x.SetExecutionPolicyAsync());
            });

            It("installs apps via scoop matching the install environment", () =>
            {
                GetMock<IScoopInstaller>().Verify(x => x.InstallAsync(scoopApps[0].AppId));
                GetMock<IScoopInstaller>().VerifyNever(x => x.InstallAsync(scoopApps[1].AppId));
                GetMock<IScoopInstaller>().Verify(x => x.InstallAsync(scoopApps[2].AppId));
            });
        }
    }
}
