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

            IArguments? capturedArguments = null;
            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(IsAny<IArguments>()))
                .Callback<IArguments>(arguments => capturedArguments = arguments)
                .ReturnsAsync(serviceProviderMock.Object);

            var result = await BecauseAsync(() => ClassUnderTest.LaunchAsync());

            It("populates arguments correctly", () =>
            {
                capturedArguments.ShouldNotBeNull().ShouldSatisfyAllConditions(x => {
                    x.ManifestPath.ShouldBe(Arguments.Default.ManifestPath);
                    x.Environments.ShouldBe(Arguments.Default.Environments);
                    x.DownloadsDir.ShouldBe(Arguments.Default.DownloadsDir);
                });
            });

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

            IArguments? capturedArguments = null;
            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(IsAny<IArguments>()))
                .Callback<IArguments>(arguments => capturedArguments = arguments)
                .ReturnsAsync(serviceProviderMock.Object);

            var result = await BecauseAsync(() => ClassUnderTest.LaunchAsync(commandlineArgs));

            It("populates arguments correctly", () =>
            {
                capturedArguments.ShouldNotBeNull().ShouldSatisfyAllConditions(x => {
                    x.ManifestPath.ShouldBe(commandlineArgs[1]);
                    x.Environments.ShouldBe(Arguments.Default.Environments);
                    x.DownloadsDir.ShouldBe(Arguments.Default.DownloadsDir);
                });
            });

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

            IArguments? capturedArguments = null;
            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(IsAny<IArguments>()))
                .Callback<IArguments>(arguments => capturedArguments = arguments)
                .ReturnsAsync(serviceProviderMock.Object);

            var result = await BecauseAsync(() => ClassUnderTest.LaunchAsync(commandlineArgs));

            It("populates arguments correctly", () =>
            {
                capturedArguments.ShouldNotBeNull().ShouldSatisfyAllConditions(x => {
                    x.ManifestPath.ShouldBe(commandlineArgs[1]);
                    x.Environments.ShouldBe(Arguments.Default.Environments);
                    x.DownloadsDir.ShouldBe(Arguments.Default.DownloadsDir);
                });
            });

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

            IArguments? capturedArguments = null;
            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(IsAny<IArguments>()))
                .Callback<IArguments>(arguments => capturedArguments = arguments)
                .ReturnsAsync(serviceProviderMock.Object);

            var result = await BecauseAsync(() => ClassUnderTest.LaunchAsync(commandlineArgs));

            It("populates arguments correctly", () =>
            {
                capturedArguments.ShouldNotBeNull().ShouldSatisfyAllConditions(x => {
                    x.ManifestPath.ShouldBe(Arguments.Default.ManifestPath);
                    x.Environments.ShouldBe(new List<string>{commandlineArgs[1]});
                    x.DownloadsDir.ShouldBe(Arguments.Default.DownloadsDir);
                });
            });

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

            IArguments? capturedArguments = null;
            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(IsAny<IArguments>()))
                .Callback<IArguments>(arguments => capturedArguments = arguments)
                .ReturnsAsync(serviceProviderMock.Object);

            var result = await BecauseAsync(() => ClassUnderTest.LaunchAsync(commandlineArgs));

            It("populates arguments correctly", () =>
            {
                capturedArguments.ShouldNotBeNull().ShouldSatisfyAllConditions(x => {
                    x.ManifestPath.ShouldBe(Arguments.Default.ManifestPath);
                    x.Environments.ShouldBe(new List<string>{commandlineArgs[1]});
                    x.DownloadsDir.ShouldBe(Arguments.Default.DownloadsDir);
                });
            });

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

            IArguments? capturedArguments = null;
            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(IsAny<IArguments>()))
                .Callback<IArguments>(arguments => capturedArguments = arguments)
                .ReturnsAsync(serviceProviderMock.Object);

            var result = await BecauseAsync(() => ClassUnderTest.LaunchAsync(commandlineArgs));

            It("populates arguments correctly", () =>
            {
                capturedArguments.ShouldNotBeNull().ShouldSatisfyAllConditions(x => {
                    x.ManifestPath.ShouldBe(Arguments.Default.ManifestPath);
                    x.Environments.ShouldBe(new List<string>{env1, env2});
                    x.DownloadsDir.ShouldBe(Arguments.Default.DownloadsDir);
                });
            });

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

            IArguments? capturedArguments = null;
            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(IsAny<IArguments>()))
                .Callback<IArguments>(arguments => capturedArguments = arguments)
                .ReturnsAsync(serviceProviderMock.Object);

            var result = await BecauseAsync(() => ClassUnderTest.LaunchAsync(commandlineArgs));

            It("populates arguments correctly", () =>
            {
                capturedArguments.ShouldNotBeNull().ShouldSatisfyAllConditions(x => {
                    x.ManifestPath.ShouldBe(Arguments.Default.ManifestPath);
                    x.Environments.ShouldBe(Arguments.Default.Environments);
                    x.DownloadsDir.ShouldBe(commandlineArgs[1]);
                });
            });

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

            IArguments? capturedArguments = null;
            GetMock<IDependencyBootstrapper>().Setup(x => x.InitializeAsync(IsAny<IArguments>()))
                .Callback<IArguments>(arguments => capturedArguments = arguments)
                .ReturnsAsync(serviceProviderMock.Object);

            var result = await BecauseAsync(() => ClassUnderTest.LaunchAsync(commandlineArgs));

            It("populates arguments correctly", () =>
            {
                capturedArguments.ShouldNotBeNull().ShouldSatisfyAllConditions(x => {
                    x.ManifestPath.ShouldBe(Arguments.Default.ManifestPath);
                    x.Environments.ShouldBe(Arguments.Default.Environments);
                    x.DownloadsDir.ShouldBe(commandlineArgs[1]);
                });
            });

            It("returns a success result", () => result.ShouldBe(0));
        }
    }
}
