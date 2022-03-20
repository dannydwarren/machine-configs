using System.Threading.Tasks;
using Configurator.Utilities;
using Configurator.Windows;
using Microsoft.Extensions.DependencyInjection;
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
        public async Task When_initializing_with_no_args()
        {
            var services = await BecauseAsync(() => ClassUnderTest.InitializeAsync(Arguments.Default));

            It("initializes arguments", () =>
            {
                services.ShouldNotBeNull().GetRequiredService<IArguments>().ShouldBe(Arguments.Default);
            });

            It("initializes static dependencies", () =>
            {
                RegistrySettingValueDataConverter.Tokenizer.ShouldNotBeNull();
            });
        }
    }
}
