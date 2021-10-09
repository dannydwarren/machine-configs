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
            var expectedValue = $"Time of test: {DateTimeOffset.Now:O}";

            Because(() => ClassUnderTest.SetValue(keyName, valueName, expectedValue));

            var actualValue = ClassUnderTest.GetValue(keyName, valueName);

            It("retrieves the value", () =>
            {
                actualValue.ShouldBe(expectedValue);
            });
        }

        [Fact]
        public void When_setting_the_value_of_a_registry_key_with_a_uint_larger_then_int()
        {
            var keyName = $@"HKEY_CURRENT_USER\SOFTWARE\{nameof(Configurator)}.{nameof(RegistryRepositoryTests)}";
            var valueName = $"{nameof(When_setting_the_value_of_a_registry_key_with_a_uint_larger_then_int)}_at_{DateTimeOffset.Now:O}";
            uint expectedValue = 4285226065;

            Because(() => ClassUnderTest.SetValue(keyName, valueName, expectedValue));

            var actualValue = ClassUnderTest.GetValue(keyName, valueName);

            It("retrieves the value", () =>
            {
                actualValue.ShouldBe(expectedValue.ToString());
            });
        }
    }
}
