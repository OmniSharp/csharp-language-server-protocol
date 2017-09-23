using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Abstractions;

namespace OmniSharp.Extensions.LanguageServer
{
    class HandlerCollection : IHandlerCollection
    {
        internal readonly HashSet<HandlerDescriptor> _handlers = new HashSet<HandlerDescriptor>();

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
                    () => _handlers.RemoveWhere(instance => instance.Handler == handler));

                handlers.Add(h);
            }

            foreach (var a in handlers)
                _handlers.Add(a);

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

        private static readonly Type[] HandlerTypes = { typeof(INotificationHandler), typeof(INotificationHandler<>), typeof(IRequestHandler<>), typeof(IRequestHandler<,>), };

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
                ?.GetTypeInfo()
                ?.GetGenericArguments()[0];
        }
    }
}