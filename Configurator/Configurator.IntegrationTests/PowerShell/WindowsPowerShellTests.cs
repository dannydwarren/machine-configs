using Configurator.PowerShell;
using Xunit;

namespace Configurator.IntegrationTests.PowerShell
{
    public class WindowsPowerShellTests : IntegrationTestBase<WindowsPowerShell>
    {
        [Fact]
        public void When_executing()
        {
            var script = @"Write-Output 'Hello World'";

            Because(() => ClassUnderTest.Execute(script));

            It("runs with out throwing an exception", () => { });
        }
    }
}
