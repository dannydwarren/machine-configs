using System;
using System.Collections.Generic;
using System.Linq;
using Configurator.Apps;
using Configurator.Installers;
using Configurator.Utilities;
using Configurator.Windows;
using Shouldly;
using Xunit;

namespace Configurator.UnitTests.Installers
{
    public class AppConfiguratorTests : UnitTestBase<AppConfigurator>
    {
        [Fact]
        public void When_configuring_registry_settings_for_an_app()
        {
            var configuration = new AppConfiguration
            {
                RegistrySettings = new List<RegistrySetting>
                {
                    new RegistrySetting { KeyName = RandomString(), ValueName = RandomString(), ValueData = RandomString() },
                    new RegistrySetting { KeyName = RandomString(), ValueName = RandomString(), ValueData = RandomString() }
                }
            };

            var mockApp = GetMock<IApp>();
            mockApp.SetupGet(x => x.Configuration).Returns(configuration);
            var app = mockApp.Object;

            Because(() => ClassUnderTest.Configure(app));

            It("sets all provided registry settings", () =>
            {
                configuration.RegistrySettings.ForEach(setting =>
                    GetMock<IRegistryRepository>()
                        .Verify(x => x.SetValue(setting.KeyName, setting.ValueName, setting.ValueData)));
            });
        }

        [Fact]
        public void When_no_configuration_is_provided()
        {
            var mockApp = GetMock<IApp>();
            mockApp.SetupGet(x => x.Configuration).Returns((AppConfiguration)null!);
            var app = mockApp.Object;

            Because(() => ClassUnderTest.Configure(app));

            It("doesn't do anything", () =>
            {
                GetMock<IRegistryRepository>()
                    .VerifyNever(x => x.SetValue(IsAny<string>(),IsAny<string>(),IsAny<string>()));
            });
        }

        [Fact]
        public void When_a_default_configuration_is_provided()
        {
            var mockApp = GetMock<IApp>();
            mockApp.SetupGet(x => x.Configuration).Returns(new AppConfiguration());
            var app = mockApp.Object;

            Because(() => ClassUnderTest.Configure(app));

            It("doesn't do anything", () =>
            {
                GetMock<IRegistryRepository>()
                    .VerifyNever(x => x.SetValue(IsAny<string>(),IsAny<string>(),IsAny<string>()));
            });
        }

        [Fact]
        public void When_setting_a_value_throws()
        {
            var configuration = new AppConfiguration
            {
                RegistrySettings = new List<RegistrySetting>
                {
                    new RegistrySetting { KeyName = RandomString(), ValueName = RandomString(), ValueData = RandomString() },
                }
            };

            var mockApp = GetMock<IApp>();
            mockApp.SetupGet(x => x.Configuration).Returns(configuration);
            var app = mockApp.Object;

            GetMock<IRegistryRepository>().Setup(x => x.SetValue(IsAny<string>(), IsAny<string>(), IsAny<object>()))
                .Throws<Exception>();

            string? capturedMessage = null;
            Exception? capturedException = null;

            GetMock<IConsoleLogger>().Setup(x => x.Error(IsAny<string>(), IsAny<Exception>()))
                .Callback<string, Exception>((message, ex) =>
                {
                    capturedMessage = message;
                    capturedException = ex;
                });

            var exception = BecauseThrows<Exception>(() => ClassUnderTest.Configure(app));

            It("notifies which Registry Setting failed", () =>
            {
                capturedMessage.ShouldNotBeNull().ShouldSatisfyAllConditions(x =>
                {
                    x.ShouldContain(configuration.RegistrySettings.Single().KeyName);
                    x.ShouldContain(configuration.RegistrySettings.Single().ValueName);
                });
            });

            It("logs the exception", () =>
            {
                capturedException.ShouldBe(exception);
            });
        }
    }
}
