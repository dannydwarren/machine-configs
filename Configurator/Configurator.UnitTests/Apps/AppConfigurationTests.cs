using Configurator.Apps;
using Shouldly;
using Xunit;

namespace Configurator.UnitTests.Apps
{
    public class AppConfigurationTests : UnitTestBase<AppConfiguration>
    {
        [Fact]
        public void When_instantiating()
        {
            var instanceUnderTest = Because(() => new AppConfiguration());

            It("does not have any null enumerables", () =>
            {
                instanceUnderTest.RegistrySettings.ShouldBeEmpty();
            });
        }
    }
}
