using System;

namespace Configurator
{
    [Flags]
    public enum InstallEnvironment
    {
        Personal = 1 << 0,
        Work = 1 << 1,
        MediaServer = 1 << 2,
        All = Personal | Work | MediaServer
    }
}
