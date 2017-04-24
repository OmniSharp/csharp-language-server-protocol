using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using JsonRpc;

namespace Lsp
{
    public static class LspHelper
    {
        private static readonly ConcurrentDictionary<Type, string> MethodNames = new ConcurrentDictionary<Type, string>();

        public static string GetMethodName<T>()
            where T : IJsonRpcHandler
        {
            return GetMethodName(typeof(T));
        }

        public static string GetMethodName(Type type)
        {
            if (MethodNames.TryGetValue(type, out var method)) return method;

            // Custom method
            var attribute = type.GetTypeInfo().GetCustomAttribute<MethodAttribute>();
            if (attribute is null)
            {
                attribute = type.GetTypeInfo()
                    .ImplementedInterfaces
                    .Select(t => t.GetTypeInfo().GetCustomAttribute<MethodAttribute>())
                    .FirstOrDefault(x => x != null);
            }

            // TODO: Log unknown method name
            if (attribute is null)
            {
                return null;
            }

            MethodNames.TryAdd(type, attribute.Method);
            return attribute.Method;
        }
    }
}