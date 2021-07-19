using System;

namespace Configurator
{
    [Flags]
    public enum InstallEnvironment
    {
        Personal = 1 << 0,
        Work = 1 << 1,
        All = Personal | Work
    }
}
