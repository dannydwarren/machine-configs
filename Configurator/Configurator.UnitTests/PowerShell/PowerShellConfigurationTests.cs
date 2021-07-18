using Configurator.Configuration;
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
            var result = new PowerShellResult
            {
                AsString = RandomString()
            };
            
            GetMock<IPowerShell>().Setup(x => x.ExecuteAsync(IsAny<string>())).ReturnsAsync(result);

            var policy = await BecauseAsync(() => ClassUnderTest.SetExecutionPolicyAsync());

            It("reports the set policy", () =>
            {
                GetMock<IConsoleLogger>().Verify(x => x.Result($"Execution Policy: {result.AsString}"));
                policy.ShouldBe(result.AsString);
            });
        }
    }
}
