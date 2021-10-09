using System;
using Configurator.Utilities;
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
        private readonly IConsoleLogger logger;

        public RegistryRepository(IConsoleLogger logger)
        {
            this.logger = logger;
        }

        public string GetValue(string keyName, string valueName)
        {
            var value = Registry.GetValue(keyName, valueName, "");

            if (value is int intValue)
            {
                value = BitConverter.ToUInt32(BitConverter.GetBytes(intValue), 0);
            }

            if (value is long longValue)
            {
                value = BitConverter.ToUInt64(BitConverter.GetBytes(longValue), 0);
            }

            return value!.ToString()!;
        }

        public void SetValue(string keyName, string valueName, object value)
        {
            var registryValueKind = value.GetType() switch
            {
                { } x when x == typeof(uint) => RegistryValueKind.DWord,
                { } x when x == typeof(string) => RegistryValueKind.String,
                _ => throw new Exception(
                    $"{nameof(RegistrySetting)}.{nameof(RegistrySetting.ValueData)} only supports types: string and uint32")
            };

            if (value is uint)
            {
                value = unchecked((int)(uint)value);
            }

            try
            {
                Registry.SetValue(keyName, valueName, value, registryValueKind);
            }
            catch (Exception e)
            {
                logger.Error(
                    $"Error setting value in registry >> {nameof(keyName)}: {keyName}; {nameof(valueName)}: {valueName}; {nameof(RegistryValueKind)}: {registryValueKind}",
                    e);
                throw;
            }
        }
    }
}
