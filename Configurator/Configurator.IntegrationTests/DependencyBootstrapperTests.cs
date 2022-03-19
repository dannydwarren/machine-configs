using System.Threading.Tasks;
using Configurator.Windows;
using Shouldly;
using Xunit;

namespace Configurator.IntegrationTests
{
    public class DependencyBootstrapperTests : IntegrationTestBase<DependencyBootstrapper>
    {
        private DependencyBootstrapper? classUnderTest;
        private new DependencyBootstrapper ClassUnderTest
        {
            get
            {
                return  classUnderTest ??= new DependencyBootstrapper(Services);
            }
        }

        [Fact]
        public async Task When_initializing()
        {
            var services = await BecauseAsync(() => ClassUnderTest.InitializeAsync());

            It("initializes all dependencies", () =>
            {
                services.ShouldNotBeNull();

                RegistrySettingValueDataConverter.Tokenizer.ShouldNotBeNull();
            });
        }
    }
}
