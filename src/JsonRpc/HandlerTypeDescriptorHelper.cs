﻿using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class HandlerTypeDescriptorHelper
    {
        private static readonly ConcurrentDictionary<Type, string> MethodNames =
            new ConcurrentDictionary<Type, string>();

        internal static readonly ImmutableSortedDictionary<string, IHandlerTypeDescriptor> KnownHandlers;

        static HandlerTypeDescriptorHelper()
        {
            try
            {
                KnownHandlers = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => {
                        try
                        {
                            return x.GetTypes();
                        }
                        catch
                        {
                            return Enumerable.Empty<Type>();
                        }
                    })
                    .Where(z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z))
                    .Where(z => z.GetCustomAttributes<MethodAttribute>().Any())
                    .Select(GetMethodType)
                    .Distinct()
                    .ToLookup(x => x.GetCustomAttribute<MethodAttribute>().Method)
                    .Select(x => new HandlerTypeDescriptor(x.First()) as IHandlerTypeDescriptor)
                    .ToImmutableSortedDictionary(x => x.Method, x => x, StringComparer.Ordinal);
            }
            catch (Exception e)
            {
                throw new AggregateException($"Failed", e);
            }
        }

        public static IHandlerTypeDescriptor GetHandlerTypeDescriptor(string method)
        {
            return KnownHandlers.TryGetValue(method, out var descriptor) ? descriptor : null;
        }

        public static IHandlerTypeDescriptor GetHandlerTypeDescriptor<T>()
        {
            return KnownHandlers.Values.FirstOrDefault(x => x.InterfaceType == typeof(T)) ??
                   GetHandlerTypeDescriptor(GetMethodName(typeof(T)));
        }

        public static IHandlerTypeDescriptor GetHandlerTypeDescriptor(Type type)
        {
            var @default = KnownHandlers.Values.FirstOrDefault(x => x.InterfaceType == type);
            if (@default != null)
            {
                return @default;
            }

            var methodName = GetMethodName(type);
            if (string.IsNullOrWhiteSpace(methodName)) return null;
            return GetHandlerTypeDescriptor(methodName);
        }

        public static string GetMethodName<T>()
            where T : IJsonRpcHandler
        {
            return GetMethodName(typeof(T));
        }

        public static bool IsMethodName(string name, params Type[] types)
        {
            return types.Any(z => GetMethodName(z).Equals(name));
        }

        public static string GetMethodName(Type type)
        {
            if (MethodNames.TryGetValue(type, out var method)) return method;

            // Custom method
            var attribute = type.GetCustomAttribute<MethodAttribute>();
            if (attribute is null)
            {
                attribute = type
                    .GetInterfaces()
                    .Select(t => t.GetCustomAttribute<MethodAttribute>())
                    .FirstOrDefault(x => x != null);
            }

            var handler = KnownHandlers.Values.FirstOrDefault(z =>
                z.InterfaceType == type || z.HandlerType == type || z.ParamsType == type);
            if (handler != null)
            {
                return handler.Method;
            }


            // TODO: Log unknown method name
            if (attribute is null)
            {
                return null;
            }

            MethodNames.TryAdd(type, attribute.Method);
            return attribute.Method;
        }

        internal static Type GetMethodType(Type type)
        {
            // Custom method
            if (type.GetTypeInfo().GetCustomAttributes<MethodAttribute>().Any())
            {
                return type;
            }

            return type.GetTypeInfo()
                .ImplementedInterfaces
                .FirstOrDefault(t => t.GetCustomAttributes<MethodAttribute>().Any());
        }

        private static readonly Type[] HandlerTypes = { typeof(IJsonRpcNotificationHandler), typeof(IJsonRpcNotificationHandler<>), typeof(IJsonRpcRequestHandler<>), typeof(IJsonRpcRequestHandler<,>), };

        private static bool IsValidInterface(Type type)
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                return HandlerTypes.Contains(type.GetGenericTypeDefinition());
            }
            return HandlerTypes.Contains(type);
        }

        public static Type GetHandlerInterface(Type type)
        {
            if (IsValidInterface(type)) return type;
            return type?.GetTypeInfo()
                .ImplementedInterfaces
                .First(IsValidInterface);
        }

        public static Type UnwrapGenericType(Type genericType, Type type)
        {
            return type?.GetTypeInfo()
                .ImplementedInterfaces
                .FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetTypeInfo().GetGenericTypeDefinition() == genericType)
                ?.GetTypeInfo()
                ?.GetGenericArguments()[0];
        }
    }
}
