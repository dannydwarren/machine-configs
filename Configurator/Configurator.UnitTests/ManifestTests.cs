using Shouldly;
using Xunit;

namespace Configurator.UnitTests
{
    public class ManifestTests : UnitTestBase<Manifest>
    {
        [Fact]
        public void When_instantiating()
        {
            var instanceUnderTest = Because(() => new Manifest());

            It("does not have any null enumerables", () =>
            {
                instanceUnderTest.Apps.ShouldBeEmpty();
            });
        }
    }
}
