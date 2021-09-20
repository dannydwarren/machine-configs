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
            Registry.SetValue(keyName, valueName, value);
        }
    }
}
