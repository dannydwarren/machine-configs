using System.Collections.Generic;
using Configurator.Windows;

namespace Configurator.Apps
{
    public class AppConfiguration
    {
        public List<RegistrySetting> RegistrySettings { get; set; } = new List<RegistrySetting>();
    }
}
