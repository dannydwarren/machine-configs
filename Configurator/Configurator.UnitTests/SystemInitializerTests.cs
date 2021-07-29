using System.Threading.Tasks;
using Configurator.Installers;
using Configurator.PowerShell;
using Xunit;

namespace Configurator.UnitTests
{
    public class SystemInitializerTests : UnitTestBase<SystemInitializer>
    {
        [Fact]
        public async Task When_initializing_the_system()
        {
            await BecauseAsync(() => ClassUnderTest.InitializeAsync());

            It("sets the PowerShell execution policy", () =>
            {
                GetMock<IPowerShellConfiguration>().Verify(x => x.SetExecutionPolicyAsync());
            });

            It("installs winget-cli", () =>
            {
                GetMock<IWingetCliInstaller>().Verify(x => x.InstallAsync());
            });

            It("installs scoop-cli", () =>
            {
                GetMock<IScoopCliInstaller>().Verify(x => x.InstallAsync());
            });
        }
    }
}
