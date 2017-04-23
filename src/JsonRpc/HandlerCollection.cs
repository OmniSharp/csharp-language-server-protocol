using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace JsonRpc
{
    class HandlerCollection : IEnumerable<IHandlerInstance>
    {
        internal readonly List<HandlerInstance> _handlers = new List<HandlerInstance>();

        internal class HandlerInstance : IHandlerInstance, IDisposable
        {
            private readonly Action _disposeAction;

            public HandlerInstance(string method, IJsonRpcHandler handler, Type handlerInterface, Type @params, Action disposeAction)
            {
                _disposeAction = disposeAction;
                Handler = handler;
                Method = method;
                HandlerType = handlerInterface;
                Params = @params;
            }

            public IJsonRpcHandler Handler { get; }
            public Type HandlerType { get; }
            public string Method { get; }
            public Type Params { get; }

            public void Dispose()
            {
                _disposeAction();
            }
        }

        public IEnumerator<IHandlerInstance> GetEnumerator()
        {
            return _handlers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Remove(IJsonRpcHandler handler)
        {
            var i = _handlers.Find(instance => instance.Handler == handler);
            if (i != null) _handlers.Remove(i);
        }

        public IDisposable Add(IJsonRpcHandler handler)
        {
            var type = handler.GetType();
            var @interface = GetHandlerInterface(type);

            Type @params = null;
            if (@interface.GetTypeInfo().IsGenericType)
            {
                @params = @interface.GetTypeInfo().GetGenericArguments()[0];
            }

            var h = new HandlerInstance(GetMethodName(type), handler, @interface, @params, () => Remove(handler));
            _handlers.Add(h);
            return h;
        }

        public IHandlerInstance Get(IJsonRpcHandler handler)
        {
            return _handlers.Find(instance => instance.Handler == handler);
        }

        public IHandlerInstance Get(string method)
        {
            return _handlers.Find(instance => instance.Method == method);
        }

        private static readonly ImmutableHashSet<Type> _handlerTypes = ImmutableHashSet.Create<Type>()
            .Add(typeof(INotificationHandler))
            .Add(typeof(INotificationHandler<>))
            .Add(typeof(IRequestHandler<>))
            .Add(typeof(IRequestHandler<,>));

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
            return type?.GetTypeInfo()
                .ImplementedInterfaces
                .First(IsValidInterface);
        }
    }
}