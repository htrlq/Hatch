using System;
using System.Collections.Concurrent;

namespace LoggerUtil
{
    public class SeriLoggerConfiguraBuild: ConcurrentDictionary<Type, SeriLoggerConfigure>
    {
    }
}
