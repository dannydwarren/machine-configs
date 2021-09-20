using System;
using Configurator.Windows;
using Shouldly;
using Xunit;

namespace Configurator.IntegrationTests.Windows
{
    public class RegistryRepositoryTests : IntegrationTestBase<RegistryRepository>
    {
        [Fact]
        public void When_getting_the_value_of_a_registry_key()
        {
            var value = Because(() => ClassUnderTest.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion", "ProgramFilesDir (x86)"));

            It("retrieves the value", () =>
            {
                value.ShouldBe(@"C:\Program Files (x86)");
            });
        }

        [Fact]
        public void When_setting_the_value_of_a_registry_key()
        {
            var keyName = $@"HKEY_CURRENT_USER\SOFTWARE\{nameof(Configurator)}.{nameof(RegistryRepositoryTests)}";
            var valueName = nameof(When_setting_the_value_of_a_registry_key);
            var expectedValue = $"Time of test: {DateTimeOffset.Now:yyyy-MM-dd hh:mm:ss:fffff}";

            Because(() => ClassUnderTest.SetValue(keyName, valueName, expectedValue));

            var actualValue = ClassUnderTest.GetValue(keyName, valueName);

            It("retrieves the value", () =>
            {
                actualValue.ShouldBe(expectedValue);
            });
        }
    }
}
