using Configurator.PowerShell;
using Moq;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Configurator.UnitTests.PowerShell
{
    public class PowerShellConfigurationTests : UnitTestBase<PowerShellConfiguration>
    {
        [Fact]
        public async Task When_setting_execution_policy()
        {
            var expectedPolicy = RandomString();
            
            GetMock<IPowerShell>().Setup(x => x.ExecuteAsync(IsAny<string>())).ReturnsAsync(expectedPolicy);

            var policy = await BecauseAsync(() => ClassUnderTest.SetExecutionPolicyAsync());

            It("reports the set policy", () =>
            {
                policy.ShouldBe(expectedPolicy);
            });
        }
    }
}
