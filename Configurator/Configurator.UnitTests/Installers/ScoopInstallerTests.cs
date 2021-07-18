using Configurator.Installers;
using Configurator.PowerShell;
using System.Threading.Tasks;
using Xunit;

namespace Configurator.UnitTests.Installers
{
    public class ScoopInstallerTests : UnitTestBase<ScoopInstaller>
    {
        [Fact]
        public async Task When_installing()
        {
            var appId = RandomString();

            await BecauseAsync(() => ClassUnderTest.InstallAsync(appId));

            It("invokes scoop via powershell", () =>
            {
                GetMock<IPowerShell>().Verify(x => x.ExecuteAsync($"scoop install {appId}"));
            });
        }
    }
}
