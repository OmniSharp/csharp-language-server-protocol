using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;

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

        public void Remove(IJsonRpcHandler handler)
        {
            _handlers.RemoveAll(instance => instance.Handler == handler);
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
                    () => Remove(handler));

                handlers.Add(h);
            }

            _handlers.AddRange(handlers);
            return new CompositeDisposable(handlers);
        }

        class CompositeDisposable : IDisposable
        {
            private readonly IEnumerable<HandlerInstance> _instances;

            public CompositeDisposable(IEnumerable<HandlerInstance> instances)
            {
                _instances = instances;
            }
            public void Dispose()
            {
                foreach (var instance in _instances)
                {
                    instance.Dispose();
                }
            }
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

    class HandlerInstance : ILspHandlerInstance, IDisposable
    {
        private readonly Action _disposeAction;

        public HandlerInstance(string method, IJsonRpcHandler handler, Type handlerType, Type @params, Type registrationType, Type capabilityType, Action disposeAction)
        {
            _disposeAction = disposeAction;
            Handler = handler;
            Method = method;
            HandlerType = handlerType;
            Params = @params;
            RegistrationType = registrationType;
            CapabilityType = capabilityType;
        }

        public IJsonRpcHandler Handler { get; }
        public Type HandlerType { get; }

        public bool HasRegistration => RegistrationType != null;
        public Type RegistrationType { get; }

        public bool HasCapability => CapabilityType != null;
        public Type CapabilityType { get; }

        private Registration _registration;

        public Registration Registration
        {
            get {
                if (_registration != null) return _registration;

                // TODO: Cache this
                var options = GetType()
                    .GetTypeInfo()
                    .GetMethod(nameof(GetRegistration))
                    .MakeGenericMethod(RegistrationType)
                    .Invoke(Handler, new object[] { Handler });

                return _registration = new Registration() {
                    Id = Guid.NewGuid().ToString(),
                    Method = Method,
                    RegisterOptions = options
                };
            }
        }

        public void SetCapability(object instance)
        {
            // TODO: Cache this
            GetType()
                .GetTypeInfo()
                .GetMethod(nameof(SetCapability))
                .MakeGenericMethod(CapabilityType)
                .Invoke(Handler, new[] { Handler, instance });

            if (instance is DynamicCapability dc)
            {
                AllowsDynamicRegistration = dc.DynamicRegistration;
            }
        }

        public string Method { get; }
        public Type Params { get; }

        public bool IsDynamicCapability => typeof(DynamicCapability).IsAssignableFrom(CapabilityType);
        public bool AllowsDynamicRegistration { get; private set; }

        public void Dispose()
        {
            _disposeAction();
        }

        private static object GetRegistration<T>(IRegistration<T> registration)
        {
            return registration.GetRegistrationOptions();
        }

        private static void SetCapability<T>(ICapability<T> capability, T instance)
        {
            capability.SetCapability(instance);
        }
    }
}