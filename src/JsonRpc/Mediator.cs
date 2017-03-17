using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JsonRPC.Server;

namespace JsonRPC
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
            internal HandlerMethod(string method)
            {
                Method = method;
                IsImplemented = false;
            }

            internal HandlerMethod(string method, Type @interface, Type handler, Type @params)
            {
                Method = method;
                IsImplemented = true;
                Interface = @interface;
                Handler = handler;
                Params = @params;
            }

            public bool IsImplemented { get; }
            public Type Interface { get; }
            public string Method { get; }
            public Type Params { get; }
            public Type Handler { get; }

            public Task Handle(object service)
            {
                var method = Interface
                    .GetType()
                    .GetMethod(nameof(INotificationHandler.Handle), BindingFlags.Public | BindingFlags.Instance);

                return (Task)method.Invoke(service, new object[0]);
            }

            public Task Handle(object service, object @params)
            {
                var method = Interface
                    .GetType()
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
            .Where(HasHandlerInterface)
            .ToArray();

            _methods = handlers
                .GroupBy(GetMethodName)
                .Select(x => {
                    var implementation = x
                        .FirstOrDefault(z => z.GetTypeInfo().IsClass);

                    if (implementation is null)
                    {
                        return new HandlerMethod(x.Key);
                    }

                    var @interface = GetHandlerInterface(implementation);
                    Type @params = null;
                    if (@interface.GetTypeInfo().IsGenericType)
                    {
                        @params = @interface.GetTypeInfo().GetGenericArguments()[0];
                    }

                    return new HandlerMethod(x.Key, @interface, implementation, @params);
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
            return type.GetTypeInfo()
                .ImplementedInterfaces
                .First(IsValidInterface);
        }
    }

    public class Mediator : IMediator
    {
        private readonly HandlerResolver _resolver;
        private readonly IServiceProvider _serviceProvider;

        public Mediator(HandlerResolver resolver, IServiceProvider serviceProvider)
        {
            _resolver = resolver;
            _serviceProvider = serviceProvider;
        }

        public async void HandleNotification(Notification notification)
        {
            var method = _resolver.GetMethod(notification.Method);

            // Q: Should be report notification not found?
            if (method is null || !method.IsImplemented) return;


            var service = _serviceProvider.GetService(method.Handler);

            // TODO: Try / catch for Internal Error

            Task result;
            if (method.Params is null)
            {
                result = method.Handle(service);
            }
            else
            {
                var @params = notification.Params.ToObject(method.Params);
                result = method.Handle(service, @params);
            }

            await result;
        }

        public async Task<ErrorResponse> HandleRequest(Request request)
        {
            var method = _resolver.GetMethod(request.Method);
            if (method is null) return new MethodNotFound(request.Id);
            if (!method.IsImplemented) return new InternalError(request.Id);

            var service = _serviceProvider.GetService(method.Handler);

            // TODO: Try / catch for Internal Error

            Task result;
            if (method.Params is null)
            {
                result = method.Handle(service);
            }
            else
            {
                object @params;
                try
                {
                    @params = request.Params.ToObject(method.Params);
                }
                catch
                {
                    return new InvalidParams(request.Id);
                }

                result = method.Handle(service, @params);
            }

            await result;

            object responseValue = null;
            if (result.GetType().GetTypeInfo().IsGenericType)
            {
                var property = typeof(Task<>)
                    .MakeGenericType(result.GetType().GetTypeInfo().GetGenericArguments()[0])
                    .GetProperty(nameof(Task<object>.Result), BindingFlags.Public | BindingFlags.Instance);

                responseValue = property.GetValue(result);
            }

            return new Response(request.Id, responseValue);
        }

        public Task SendNotification<T>(string method, T @params)
        {
            throw new NotImplementedException();
        }

        public Task<TResponse> SendRequest<T, TResponse>(string method, T @params)
        {
            throw new NotImplementedException();
        }

        public Task SendRequest<T>(string method, T @params)
        {
            throw new NotImplementedException();
        }
    }
}