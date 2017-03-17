using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JsonRpc
{
    public class HandlerResolver
    {
        private static ImmutableHashSet<Type> _handlerTypes = ImmutableHashSet.Create<Type>()
            .Add(typeof(INotificationHandler))
            .Add(typeof(INotificationHandler<>))
            .Add(typeof(IRequestHandler<>))
            .Add(typeof(IRequestHandler<,>));

        internal ImmutableDictionary<string, HandlerMethod> _methods { get; }
        internal ImmutableHashSet<string> _methodNames { get; }

        public class HandlerMethod
        {
            internal HandlerMethod(string method, Type serviceInterface, Type handlerInterface, Type @params)
            {
                Method = method;
                HandlerInterface = handlerInterface;
                ServiceInterface = serviceInterface;
                Params = @params;
            }

            public Type ServiceInterface { get; }

            public Type HandlerInterface { get; }
            public string Method { get; }
            public Type Params { get; }

            public Task Handle(object service)
            {
                var method = HandlerInterface
                    .GetMethod(nameof(INotificationHandler.Handle), BindingFlags.Public | BindingFlags.Instance);

                return (Task)method.Invoke(service, new object[0]);
            }

            public Task Handle(object service, object @params)
            {
                var method = HandlerInterface
                    .GetMethod(nameof(INotificationHandler.Handle), BindingFlags.Public | BindingFlags.Instance);

                return (Task)method.Invoke(service, new[] { @params });
            }
        }

        public HandlerResolver(Assembly assembly, params Assembly[] assemblies) : this(new[] { assembly }.Concat(assemblies)) { }

        public HandlerResolver(IEnumerable<Assembly> assemblies)
        {
            var handlers = assemblies
                .SelectMany(x => {
                    try
                    {
                        return x.DefinedTypes;
                    }
                    catch
                    {

                    }
                    return Enumerable.Empty<Type>();
                })
                .Where(x => x.GetTypeInfo().IsInterface)
                .Where(HasHandlerInterface)
                .ToArray();

            _methods = handlers
                .Select(x => {
                    var @interface = GetHandlerInterface(x);

                    Type @params = null;
                    if (@interface.GetTypeInfo().IsGenericType)
                    {
                        @params = @interface.GetTypeInfo().GetGenericArguments()[0];
                    }

                    return new HandlerMethod(GetMethodName(x), x, @interface, @params);
                })
                .ToImmutableDictionary(x => x.Method);
        }

        public HandlerMethod GetMethod(string name)
        {
            if (_methods.TryGetValue(name, out var result))
            {
                return result;
            }
            return null;
        }

        private string GetMethodName(Type type)
        {
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

            }

            return attribute.Method;
        }

        private bool HasHandlerInterface(Type type)
        {
            return type.GetTypeInfo()
                .ImplementedInterfaces
                .Any(IsValidInterface);
        }

        private bool IsValidInterface(Type type)
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                return _handlerTypes.Contains(type.GetGenericTypeDefinition());
            }
            return _handlerTypes.Contains(type);
        }

        private Type GetHandlerInterface(Type type)
        {
            return type
               ?.GetTypeInfo()
                .ImplementedInterfaces
                .First(IsValidInterface);
        }
    }
}