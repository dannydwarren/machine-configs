using System.Collections.Generic;
using Configurator.Apps;

namespace Configurator
{
    public class Manifest
    {
        public List<IApp> Apps { get; set; } = new List<IApp>();
    }
}
