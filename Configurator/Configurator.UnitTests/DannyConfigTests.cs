using Xunit;

namespace Configurator.UnitTests
{
    public class DannyConfigTests : UnitTestBase<DannyConfig>
    {
        [Fact]
        public void When_executing_for_all_environments()
        {
            Because(() => ClassUnderTest.Execute());

            It("?", () => { });
        }
    }
}
