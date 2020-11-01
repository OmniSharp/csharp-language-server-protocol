using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IHandlerTypeDescriptorProvider<out T> where T : IHandlerTypeDescriptor?
    {
        T GetHandlerTypeDescriptor<TA>();
        T GetHandlerTypeDescriptor(Type type);
        string? GetMethodName<TH>() where TH : IJsonRpcHandler;
        bool IsMethodName(string name, params Type[] types);
        string? GetMethodName(Type type);
    }

    public class HandlerTypeDescriptorHelper
    {
        internal static Type? GetMethodType(Type type)
        {
            // Custom method
            if (MethodAttribute.AllFrom(type).Any())
            {
                return type;
            }

            return type.GetTypeInfo()
                       .ImplementedInterfaces
                       .FirstOrDefault(t => MethodAttribute.AllFrom(t).Any());
        }

        private static readonly Type[] HandlerTypes = {
            typeof(IJsonRpcNotificationHandler<>),
            typeof(IJsonRpcRequestHandler<>),
            typeof(IJsonRpcRequestHandler<,>),
            typeof(IRequestHandler<>),
            typeof(IRequestHandler<,>),
        };

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
            try
            {
                if (IsValidInterface(type)) return type;
                return type.GetTypeInfo()
                            .ImplementedInterfaces
                            .First(IsValidInterface);
            }
            catch (Exception e)
            {
                throw new AggregateException("Errored with type: " + type.FullName, e);
            }
        }

        internal static Type? UnwrapGenericType(Type genericType, Type type) =>
            type.GetTypeInfo()
                 .ImplementedInterfaces
                 .FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetTypeInfo().GetGenericTypeDefinition() == genericType)
                ?.GetTypeInfo()
                ?.GetGenericArguments()[0];
    }

    class HandlerTypeDescriptorProvider : IHandlerTypeDescriptorProvider<IHandlerTypeDescriptor?>
    {
        private readonly ConcurrentDictionary<Type, string> _methodNames =
            new ConcurrentDictionary<Type, string>();

        internal readonly ILookup<string, IHandlerTypeDescriptor> KnownHandlers;

        internal HandlerTypeDescriptorProvider(IEnumerable<Assembly> assemblies)
        {
            try
            {
                KnownHandlers = GetDescriptors(assemblies)
                   .ToLookup(x => x.Method, StringComparer.Ordinal);
            }
            catch (Exception e)
            {
                throw new AggregateException("Failed", e);
            }
        }

        internal static IEnumerable<IHandlerTypeDescriptor> GetDescriptors(IEnumerable<Assembly> assemblies) => assemblies.SelectMany(
                x => {
                    try
                    {
                        return x.GetTypes();
                    }
                    catch
                    {
                        return Enumerable.Empty<Type>();
                    }
                }
            )
           .Where(z => z.IsInterface || z.IsClass && !z.IsAbstract)
            // running on mono this call can cause issues when scanning of the entire assembly.
           .Where(
                z => {
                    try
                    {
                        return typeof(IJsonRpcHandler).IsAssignableFrom(z);
                    }
                    catch
                    {
                        return false;
                    }
                }
            )
           .Where(z => MethodAttribute.From(z) != null)
           .Where(z => !z.Name.EndsWith("Manager")) // Manager interfaces are generally specializations around the handlers
           .Select(HandlerTypeDescriptorHelper.GetMethodType)
           .Distinct()
           .ToLookup(x => MethodAttribute.From(x)!.Method)
           .SelectMany(
                x => x
                    .Distinct()
                    .Select(z => new HandlerTypeDescriptor(z!) as IHandlerTypeDescriptor)
            );

        public IHandlerTypeDescriptor? GetHandlerTypeDescriptor<TA>() => GetHandlerTypeDescriptor(typeof(TA));

        public IHandlerTypeDescriptor? GetHandlerTypeDescriptor(Type type)
        {
            var @default = KnownHandlers
                          .SelectMany(g => g)
                          .FirstOrDefault(x => x.InterfaceType == type || x.HandlerType == type || x.ParamsType == type);
            if (@default != null)
            {
                return @default;
            }

            var methodName = GetMethodName(type)!;
            return string.IsNullOrWhiteSpace(methodName) ? null : KnownHandlers[methodName].FirstOrDefault();
        }

        public string? GetMethodName<T>() where T : IJsonRpcHandler => GetMethodName(typeof(T));

        public bool IsMethodName(string name, params Type[] types) => types.Any(z => GetMethodName(z)?.Equals(name) == true);

        public string? GetMethodName(Type type)
        {
            if (_methodNames.TryGetValue(type, out var method)) return method;

            // Custom method
            var attribute = MethodAttribute.From(type);

            var handler = KnownHandlers.SelectMany(z => z)
                                       .FirstOrDefault(z => z.InterfaceType == type || z.HandlerType == type || z.ParamsType == type);
            if (handler != null)
            {
                return handler.Method;
            }

            if (attribute is null)
            {
                return null;
            }

            _methodNames.TryAdd(type, attribute.Method);
            return attribute.Method;
        }
    }
}
