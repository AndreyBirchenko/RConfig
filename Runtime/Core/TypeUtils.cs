using System;
using System.Collections.Generic;

namespace RConfig.Runtime
{
    public static class TypeUtils
    {
        private static Dictionary<string,Type> _schemeTypesByNames = new();

        static TypeUtils()
        {
            var schemeType = typeof(RCScheme);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (schemeType.IsAssignableFrom(type) && type != schemeType)
                    {
                        _schemeTypesByNames.Add(type.Name, type);
                    }
                }
            }
        }

        public static Type GetTypeByName(string name)
        {
            if (_schemeTypesByNames.TryGetValue(name, out var value))
            {
                return value;
            }

            throw new Exception($"Can not find type by name {name}");
        }
    }
}