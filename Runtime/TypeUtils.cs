using System;
using System.Collections.Generic;

namespace RConfig.Runtime
{
    public static class TypeUtils
    {
        private static List<Type> _schemeTypesCache = new();

        static TypeUtils()
        {
            var schemeType = typeof(RCScheme);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (schemeType.IsAssignableFrom(type) && type != schemeType)
                    {
                        _schemeTypesCache.Add(type);
                    }
                }
            }
        }

        public static List<Type> GetSchemeTypes() => _schemeTypesCache;
    }
}