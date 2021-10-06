using System;
using Microsoft.Win32;

namespace Configurator.Windows
{
    public interface IRegistryRepository
    {
        string GetValue(string keyName, string valueName);
        void SetValue(string keyName, string valueName, object value);
    }

    public class RegistryRepository : IRegistryRepository
    {
        public string GetValue(string keyName, string valueName)
        {
            return Registry.GetValue(keyName, valueName, "").ToString()!;
        }

        public void SetValue(string keyName, string valueName, object value)
        {
            var registryValueKind = value.GetType() switch
            {
                { } x when x == typeof(uint) => RegistryValueKind.DWord,
                { } x when x == typeof(string) => RegistryValueKind.String,
                _ => throw new Exception($"{nameof(RegistrySetting)}.{nameof(RegistrySetting.ValueData)} only supports types: string and uint32")
            };

            Registry.SetValue(keyName, valueName, value, registryValueKind);
        }
    }
}
