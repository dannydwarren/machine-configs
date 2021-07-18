using Configurator.Installers;
using Configurator.PowerShell;
using System.Threading.Tasks;
using Xunit;

namespace Configurator.UnitTests
{
    public class DannyConfigTests : UnitTestBase<DannyConfig>
    {
        [Fact]
        public async Task When_executing_for_all_environments()
        {
            await BecauseAsync(() => ClassUnderTest.ExecuteAsync());

            It("enables scripts to be executed", () =>
            {
                GetMock<IPowerShellConfiguration>().Verify(x => x.SetExecutionPolicyAsync());
            });

            It("scoop installs mob", () =>
            {
                GetMock<IScoopInstaller>().Verify(x => x.InstallAsync("mob"));
            });
        }
    }
}
