using System;
using System.CodeDom;
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
        private readonly List<HandlerDescriptor> _handlers = new List<HandlerDescriptor>();

        public IEnumerator<ILspHandlerDescriptor> GetEnumerator()
        {
            return _handlers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IDisposable Add(IJsonRpcHandler handler)
        {
            //var type = handler.GetType();

            var handlers = new List<HandlerDescriptor>();
            foreach (var implementedInterface in handler.GetType().GetTypeInfo()
                .ImplementedInterfaces
                .Where(x => !string.IsNullOrWhiteSpace(LspHelper.GetMethodName(x))))
            {
                var @interface = GetHandlerInterface(implementedInterface);
                var registration = UnwrapGenericType(typeof(IRegistration<>), implementedInterface);
                var capability = UnwrapGenericType(typeof(ICapability<>), implementedInterface);

                Type @params = null;
                if (@interface.GetTypeInfo().IsGenericType)
                {
                    @params = @interface.GetTypeInfo().GetGenericArguments()[0];
                }

                var h = new HandlerDescriptor(
                    LspHelper.GetMethodName(implementedInterface),
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

        public IEnumerable<ILspHandlerDescriptor> Get(IJsonRpcHandler handler)
        {
            return _handlers.Where(instance => instance.Handler == handler);
        }

        public IEnumerable<ILspHandlerDescriptor> Get(string method)
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

        private Type GetHandlerInterface(Type type)
        {
            return type?.GetTypeInfo()
                .ImplementedInterfaces
                .First(IsValidInterface);
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