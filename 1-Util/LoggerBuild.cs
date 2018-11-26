using System;
using System.Collections.Concurrent;
using Serilog.Core;

namespace LoggerUtil
{
    public class LoggerBuild
    {
        private ConcurrentDictionary<Type, Logger> dictionary = new ConcurrentDictionary<Type, Logger>();

        public bool ContainsKey(Type type)
        {
            return dictionary.ContainsKey(type);
        }

        public bool Add(Type type, Logger logger)
        {
            return dictionary.TryAdd(type, logger);
        }

        public Logger this[Type key] => dictionary[key];
    }
}
