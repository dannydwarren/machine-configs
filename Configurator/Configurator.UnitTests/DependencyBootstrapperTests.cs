using System;
using System.Collections.Generic;
using System.IO;
using Configurator.Utilities;
using Configurator.Windows;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit;

namespace Configurator.UnitTests
{
    public class DependencyBootstrapperTests : UnitTestBase<DependencyBootstrapper>
    {
        [Fact]
        public void When_initializing_service_provider_with_no_args()
        {
            IArguments? capturedArgs = null;
            GetMock<IServiceCollection>().Setup(x => x.Add(IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(serviceDescriptor =>
                {
                    if (serviceDescriptor.ImplementationInstance is IArguments arguments)
                    {
                        capturedArgs = arguments;
                    }
                });

            var serviceProvider = Because(() => ClassUnderTest.InitializeServiceProvider());

            It("configures dependencies", () =>
            {
                GetMock<IServiceCollection>().Verify(x => x.Add(Moq.It.Is<ServiceDescriptor>(y => y.ServiceType == typeof(ITokenizer))), Times.Once);
                serviceProvider.ShouldNotBeNull();
            });

            It("uses default args", () =>
            {
                capturedArgs.ShouldNotBeNull().ShouldSatisfyAllConditions(x =>
                {
                    x.ManifestPath.ShouldBe("https://raw.githubusercontent.com/dannydwarren/machine-configs/main/manifests/test.manifest.json");
                    x.Environments.ShouldBe(new List<string> { "test" });
                    x.DownloadsDir.ShouldBe(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"));
                });
            });
        }

        [Fact]
        public void When_initializing_service_provider_with_custom_manifest()
        {
            string expectedManifest = RandomString();

            IArguments? capturedArgs = null;
            GetMock<IServiceCollection>().Setup(x => x.Add(IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(serviceDescriptor =>
                {
                    if (serviceDescriptor.ImplementationInstance is IArguments arguments)
                    {
                        capturedArgs = arguments;
                    }
                });

            Because(() => ClassUnderTest.InitializeServiceProvider(manifestPath: expectedManifest));

            It("uses the specified manifest", () =>
            {
                capturedArgs.ShouldNotBeNull().ManifestPath.ShouldBe(expectedManifest);
            });
        }

        [Fact]
        public void When_initializing_service_provider_with_custom_environments()
        {
            var expectedEnvironments = new List<string> { RandomString(), RandomString() };

            IArguments? capturedArgs = null;
            GetMock<IServiceCollection>().Setup(x => x.Add(IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(serviceDescriptor =>
                {
                    if (serviceDescriptor.ImplementationInstance is IArguments arguments)
                    {
                        capturedArgs = arguments;
                    }
                });

            Because(() => ClassUnderTest.InitializeServiceProvider(environments: expectedEnvironments));

            It("uses the specified manifest", () =>
            {
                capturedArgs.ShouldNotBeNull().Environments.ShouldBe(expectedEnvironments);
            });
        }

        [Fact]
        public void When_initializing_service_provider_with_custom_downloadsDir()
        {
            string expectedDownloadsDir = RandomString();

            IArguments? capturedArgs = null;
            GetMock<IServiceCollection>().Setup(x => x.Add(IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(serviceDescriptor =>
                {
                    if (serviceDescriptor.ImplementationInstance is IArguments arguments)
                    {
                        capturedArgs = arguments;
                    }
                });

            Because(() => ClassUnderTest.InitializeServiceProvider(downloadsDir: expectedDownloadsDir));

            It("uses the specified manifest", () =>
            {
                capturedArgs.ShouldNotBeNull().DownloadsDir.ShouldBe(expectedDownloadsDir);
            });
        }

        [Fact]
        public void When_initializing_static_dependencies()
        {
            var expectedTokenizer = GetMock<ITokenizer>().Object;

            var serviceProviderMock = GetMock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(ITokenizer))).Returns(expectedTokenizer);

            Because(() => ClassUnderTest.InitializeStaticDependencies(serviceProviderMock.Object));

            It("configures them", () =>
            {
                RegistrySettingValueDataConverter.Tokenizer.ShouldNotBeNull().ShouldBe(expectedTokenizer);
            });
        }
    }
}
