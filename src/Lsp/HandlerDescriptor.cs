using System;
using System.Reflection;
using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;

namespace Lsp
{
    class HandlerDescriptor : ILspHandlerDescriptor, IDisposable
    {
        private readonly Action _disposeAction;

        public HandlerDescriptor(string method, IJsonRpcHandler handler, Type handlerType, Type @params, Type registrationType, Type capabilityType, Action disposeAction)
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