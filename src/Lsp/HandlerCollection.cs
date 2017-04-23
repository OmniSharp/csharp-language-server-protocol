using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using JsonRpc;

namespace Lsp
{
    class HandlerCollection : IHandlerCollection
    {
        private readonly List<HandlerInstance> _handlers = new List<HandlerInstance>();

        public IEnumerator<ILspHandlerInstance> GetEnumerator()
        {
            return _handlers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IDisposable Add(IJsonRpcHandler handler)
        {
            var type = handler.GetType();
            var interfaces = GetHandlerInterfaces(type);

            var handlers = new List<HandlerInstance>();
            foreach (var @interface in interfaces)
            {
                var registration = UnwrapGenericType(typeof(IRegistration<>), @interface);
                var capability = UnwrapGenericType(typeof(ICapability<>), @interface);

                Type @params = null;
                if (@interface.GetTypeInfo().IsGenericType)
                {
                    @params = @interface.GetTypeInfo().GetGenericArguments()[0];
                }

                var h = new HandlerInstance(
                    LspHelper.GetMethodName(@interface),
                    handler,
                    @interface,
                    @params,
                    registration,
                    capability,
                    () => _handlers.RemoveAll(instance => instance.Handler == handler));

                handlers.Add(h);
            }

            _handlers.AddRange(handlers);
            return new ImutableDisposable(handlers);
        }

        public IEnumerable<ILspHandlerInstance> Get(IJsonRpcHandler handler)
        {
            return _handlers.Where(instance => instance.Handler == handler);
        }

        public IEnumerable<ILspHandlerInstance> Get(string method)
        {
            return _handlers.Where(instance => instance.Method == method);
        }

        private static readonly ImmutableHashSet<Type> HandlerTypes = ImmutableHashSet.Create<Type>()
            .Add(typeof(INotificationHandler))
            .Add(typeof(INotificationHandler<>))
            .Add(typeof(IRequestHandler<>))
            .Add(typeof(IRequestHandler<,>));

        private bool IsValidInterface(Type type)
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                return HandlerTypes.Contains(type.GetGenericTypeDefinition());
            }
            return HandlerTypes.Contains(type);
        }

        private IEnumerable<Type> GetHandlerInterfaces(Type type)
        {
            return type?.GetTypeInfo()
                .ImplementedInterfaces
                .Where(IsValidInterface);
        }

        private Type UnwrapGenericType(Type genericType, Type type)
        {
            return type?.GetTypeInfo()
                .ImplementedInterfaces
                .FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetTypeInfo().GetGenericTypeDefinition() == genericType)
                ?.GetGenericArguments()[0];
        }
    }
}