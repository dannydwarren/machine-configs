using System.Collections.Generic;
using Configurator.Apps;
using Configurator.Installers;
using Configurator.Windows;
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
    }
}
