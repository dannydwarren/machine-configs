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

            var serviceProvider = Because(() => ClassUnderTest.InitializeServiceProvider(Arguments.Default));

            It("configures dependencies", () =>
            {
                GetMock<IServiceCollection>().Verify(x => x.Add(Moq.It.Is<ServiceDescriptor>(y => y.ServiceType == typeof(ITokenizer))), Times.Once);
                serviceProvider.ShouldNotBeNull();
            });

            It("uses default args", () =>
            {
                capturedArgs.ShouldBe(Arguments.Default);
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
