using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Configurator.UnitTests
{
    public class CliTests : UnitTestBase<Cli>
    {
        [Fact]
        public async Task When_running_configurator_with_no_commandline_args()
        {
            var machineConfiguratorMock = GetMock<IMachineConfigurator>();

            var serviceProviderMock = GetMock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IMachineConfigurator)))
                .Returns(machineConfiguratorMock.Object);

            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(IsAny<string>(), IsAny<List<string>>(), IsAny<string>())).ReturnsAsync(serviceProviderMock.Object);

            await BecauseAsync(() => ClassUnderTest.RunConfiguratorAsync());

            It("runs machine configurator",
                () => { machineConfiguratorMock.Verify(x => x.ExecuteAsync(), Times.Once); });
        }

        [Fact]
        public async Task When_running_configurator_with_custom_commandline_args()
        {
            var machineConfiguratorMock = GetMock<IMachineConfigurator>();

            var serviceProviderMock = GetMock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IMachineConfigurator)))
                .Returns(machineConfiguratorMock.Object);

            var manifestPath = RandomString();
            var environments = new List<string> { RandomString() };
            var downloadsDir = RandomString();

            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(manifestPath, environments, downloadsDir)).ReturnsAsync(serviceProviderMock.Object);

            await BecauseAsync(() => ClassUnderTest.RunConfiguratorAsync(manifestPath, environments, downloadsDir));

            It("runs machine configurator",
                () => { machineConfiguratorMock.Verify(x => x.ExecuteAsync(), Times.Once); });
        }
    }
}
