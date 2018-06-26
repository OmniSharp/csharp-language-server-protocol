using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    [DebuggerDisplay("{Method}")]
    public class HandlerCollection : IEnumerable<IHandlerDescriptor>
    {
        internal readonly List<HandlerInstance> _handlers = new List<HandlerInstance>();

        internal class HandlerInstance : IHandlerDescriptor, IDisposable
        {
            private readonly Action _disposeAction;

            public HandlerInstance(string method, IJsonRpcHandler handler, Type handlerInterface, Type @params, Type response, Action disposeAction)
            {
                _disposeAction = disposeAction;
                Handler = handler;
                ImplementationType = handler.GetType();
                Method = method;
                HandlerType = handlerInterface;
                Params = @params;
                Response = response;
            }

            public IJsonRpcHandler Handler { get; }
            public Type HandlerType { get; }
            public Type ImplementationType { get; }
            public string Method { get; }
            public Type Params { get; }
            public Type Response { get; }

            public void Dispose()
            {
                _disposeAction();
            }
        }

        public IEnumerator<IHandlerDescriptor> GetEnumerator()
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
            return Add(GetMethodName(handler.GetType()), handler);
        }

        public IDisposable Add(string method, IJsonRpcHandler handler)
        {
            var type = handler.GetType();
            var @interface = GetHandlerInterface(type);

            Type @params = null;
            Type response = null;
            if (@interface.GetTypeInfo().IsGenericType)
            {
                @params = @interface.GetTypeInfo().GetGenericArguments()[0];
                var requestInterface = @params.GetInterfaces()
                    .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>));
                if (requestInterface != null)
                    response = requestInterface.GetGenericArguments()[0];
            }

            var h = new HandlerInstance(method, handler, @interface, @params, response, () => Remove(handler));
            _handlers.Add(h);
            return h;
        }

        private static readonly Type[] HandlerTypes = {
            typeof(IJsonRpcNotificationHandler),
            typeof(IJsonRpcNotificationHandler<>),
            typeof(IJsonRpcRequestHandler<>),
            typeof(IJsonRpcRequestHandler<,>),
        };

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
    }
}
