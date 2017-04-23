using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using JsonRpc;

namespace Lsp
{
    class HandlerCollection : IHandlerCollection
    {
        private readonly List<HandlerInstance> _handlers = new List<HandlerInstance>();

        class HandlerInstance : ILspHandlerInstance, IDisposable
        {
            private readonly Action _disposeAction;

            public HandlerInstance(string method, IJsonRpcHandler handler, Type handlerInterface, Type @params, Type registrationType, Action disposeAction)
            {
                _disposeAction = disposeAction;
                Handler = handler;
                Method = method;
                HandlerInterface = handlerInterface;
                Params = @params;
                RegistrationType = registrationType;
            }

            public IJsonRpcHandler Handler { get; }
            public Type HandlerInterface { get; }
            public Type RegistrationType{ get; }
            public string Method { get; }
            public Type Params { get; }

            public void Dispose()
            {
                _disposeAction();
            }
        }

        public IEnumerator<ILspHandlerInstance> GetEnumerator()
        {
            return _handlers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Remove(IJsonRpcHandler handler)
        {
            var i = _handlers.Find(instance => instance.Handler == handler);
            if (i != null) _handlers.Remove(i);
        }

        public IDisposable Add(IJsonRpcHandler handler)
        {
            var type = handler.GetType();
            var @interface = GetHandlerInterface(type);
            var @registration = GetRegistrationType(type);

            Type @params = null;
            if (@interface.GetTypeInfo().IsGenericType)
            {
                @params = @interface.GetTypeInfo().GetGenericArguments()[0];
            }

            var h = new HandlerInstance(LspHelper.GetMethodName(type), handler, @interface, @params, @registration, () => Remove(handler));
            _handlers.Add(h);
            return h;
        }

        public IHandlerInstance Get(IJsonRpcHandler handler)
        {
            return _handlers.Find(instance => instance.Handler == handler);
        }

        public ILspHandlerInstance Get(string method)
        {
            return _handlers.Find(instance => instance.Method == method);
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

        private Type GetRegistrationType(Type type)
        {
            return type?.GetTypeInfo()
                .ImplementedInterfaces
                .FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetTypeInfo().GetGenericTypeDefinition() == typeof(IRegistration<>))
                ?.GetGenericArguments()[0];
        }
    }
}