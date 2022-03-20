using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Configurator.Utilities;
using Moq;
using Shouldly;
using Xunit;

namespace Configurator.UnitTests
{
    public class CliTests : UnitTestBase<Cli>
    {
        [Fact]
        public async Task When_launching_with_no_commandline_args()
        {
            var machineConfiguratorMock = GetMock<IMachineConfigurator>();

            var serviceProviderMock = GetMock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IMachineConfigurator)))
                .Returns(machineConfiguratorMock.Object);

            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(Arguments.Default.ManifestPath, IsSequenceEqual(Arguments.Default.Environments), Arguments.Default.DownloadsDir)).ReturnsAsync(serviceProviderMock.Object);

            var result = await BecauseAsync(() => ClassUnderTest.LaunchAsync());

            It("runs machine configurator",
                () => { machineConfiguratorMock.Verify(x => x.ExecuteAsync(), Times.Once); });

            It("returns a success result", () => result.ShouldBe(0));
        }

        [Fact]
        public async Task When_launching_with_fully_qualified_manifest_path_commandline_args()
        {
            var machineConfiguratorMock = GetMock<IMachineConfigurator>();

            var serviceProviderMock = GetMock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IMachineConfigurator)))
                .Returns(machineConfiguratorMock.Object);

            var commandlineArgs = new[] { "--manifest-path", RandomString() };

            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(commandlineArgs[1], IsAny<List<string>>(), IsAny<string>())).ReturnsAsync(serviceProviderMock.Object);

            var result = await BecauseAsync(() => ClassUnderTest.LaunchAsync(commandlineArgs));

            It("returns a success result", () => result.ShouldBe(0));
        }

        [Fact]
        public async Task When_launching_with_short_alias_manifest_path_commandline_args()
        {
            var machineConfiguratorMock = GetMock<IMachineConfigurator>();

            var serviceProviderMock = GetMock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IMachineConfigurator)))
                .Returns(machineConfiguratorMock.Object);

            var commandlineArgs = new[] { "-m", RandomString() };

            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(commandlineArgs[1], IsAny<List<string>>(), IsAny<string>())).ReturnsAsync(serviceProviderMock.Object);

            var result = await BecauseAsync(() => ClassUnderTest.LaunchAsync(commandlineArgs));

            It("returns a success result", () => result.ShouldBe(0));
        }

        [Fact]
        public async Task When_launching_with_fully_qualified_environments_commandline_args()
        {
            var machineConfiguratorMock = GetMock<IMachineConfigurator>();

            var serviceProviderMock = GetMock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IMachineConfigurator)))
                .Returns(machineConfiguratorMock.Object);

            var commandlineArgs = new[] { "--environments", RandomString() };

            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(IsAny<string>(), IsSequenceEqual(new List<string>{commandlineArgs[1]}), IsAny<string>())).ReturnsAsync(serviceProviderMock.Object);

            var result = await BecauseAsync(() => ClassUnderTest.LaunchAsync(commandlineArgs));

            It("returns a success result", () => result.ShouldBe(0));
        }

        [Fact]
        public async Task When_launching_with_short_alias_environments_commandline_args()
        {
            var machineConfiguratorMock = GetMock<IMachineConfigurator>();

            var serviceProviderMock = GetMock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IMachineConfigurator)))
                .Returns(machineConfiguratorMock.Object);

            var commandlineArgs = new[] { "-e", RandomString() };

            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(IsAny<string>(), IsSequenceEqual(new List<string>{commandlineArgs[1]}), IsAny<string>())).ReturnsAsync(serviceProviderMock.Object);

            var result = await BecauseAsync(() => ClassUnderTest.LaunchAsync(commandlineArgs));

            It("returns a success result", () => result.ShouldBe(0));
        }

        [Fact]
        public async Task When_launching_with_multiple_environments_commandline_args()
        {
            var machineConfiguratorMock = GetMock<IMachineConfigurator>();

            var serviceProviderMock = GetMock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IMachineConfigurator)))
                .Returns(machineConfiguratorMock.Object);

            var env1 = RandomString();
            var env2 = RandomString();
            var commandlineArgs = new[] { "--environments", $"{env1}|{env2}" };

            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(IsAny<string>(), IsSequenceEqual(new List<string>{env1, env2}), IsAny<string>())).ReturnsAsync(serviceProviderMock.Object);

            var result = await BecauseAsync(() => ClassUnderTest.LaunchAsync(commandlineArgs));

            It("returns a success result", () => result.ShouldBe(0));
        }

        [Fact]
        public async Task When_launching_with_fully_qualified_downloads_dir_commandline_args()
        {
            var machineConfiguratorMock = GetMock<IMachineConfigurator>();

            var serviceProviderMock = GetMock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IMachineConfigurator)))
                .Returns(machineConfiguratorMock.Object);

            var commandlineArgs = new[] { "--downloads-dir", RandomString() };

            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(IsAny<string>(), IsAny<List<string>>(), commandlineArgs[1])).ReturnsAsync(serviceProviderMock.Object);

            var result = await BecauseAsync(() => ClassUnderTest.LaunchAsync(commandlineArgs));

            It("returns a success result", () => result.ShouldBe(0));
        }

        [Fact]
        public async Task When_launching_with_short_alias_downloads_dir_commandline_args()
        {
            var machineConfiguratorMock = GetMock<IMachineConfigurator>();

            var serviceProviderMock = GetMock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IMachineConfigurator)))
                .Returns(machineConfiguratorMock.Object);

            var commandlineArgs = new[] { "-dl", RandomString() };

            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(IsAny<string>(), IsAny<List<string>>(), commandlineArgs[1])).ReturnsAsync(serviceProviderMock.Object);

            var result = await BecauseAsync(() => ClassUnderTest.LaunchAsync(commandlineArgs));

            It("returns a success result", () => result.ShouldBe(0));
        }

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
