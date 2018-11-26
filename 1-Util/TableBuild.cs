using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Serilog.Sinks.MSSqlServer;

namespace LoggerUtil
{
    public class TableBuild
    {
        private ConcurrentDictionary<Type,List<ColumInformat>> dictionary = new ConcurrentDictionary<Type, List<ColumInformat>>();

        public bool ContainsKey(Type type)
        {
            return dictionary.ContainsKey(type);
        }

        public bool Add(Type type)
        {
            var list = new List<ColumInformat>();
            foreach (var propertyInfo in type.GetProperties())
            {
                list.Add(
                  new ColumInformat()
                  {
                      DataType = propertyInfo.PropertyType,
                      Property = propertyInfo.Name
                  }
                );
            }

            return dictionary.TryAdd(type, list);
        }

        public List<ColumInformat> this[Type key] => dictionary[key];
    }
}
