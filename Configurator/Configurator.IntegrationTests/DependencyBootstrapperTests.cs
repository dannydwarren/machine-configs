using System.Collections.Generic;
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
            var services = await BecauseAsync(() => ClassUnderTest.InitializeAsync());

            It("initializes all dependencies", () =>
            {
                services.ShouldNotBeNull();

                RegistrySettingValueDataConverter.Tokenizer.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task When_initializing_with_custom_args()
        {
            var expectedManifestPath = RandomString();
            var expectedEnvironments = new List<string> { RandomString() };
            var expectedDownloadsDir = RandomString();

            var services = await BecauseAsync(() => ClassUnderTest.InitializeAsync(expectedManifestPath, expectedEnvironments, expectedDownloadsDir));

            It("initializes arguments with custom args", () =>
            {
                services.ShouldNotBeNull().GetRequiredService<IArguments>().ShouldSatisfyAllConditions(x =>
                {
                    x.ManifestPath.ShouldBe(expectedManifestPath);
                    x.Environments.ShouldBe(expectedEnvironments);
                    x.DownloadsDir.ShouldBe(expectedDownloadsDir);
                });
            });
        }
    }
}
