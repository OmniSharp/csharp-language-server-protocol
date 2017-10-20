using System;
using System.Collections.Generic;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Abstractions;
using OmniSharp.Extensions.LanguageServer.Capabilities.Client;
using OmniSharp.Extensions.LanguageServer.Models;

namespace OmniSharp.Extensions.LanguageServer
{
    class HandlerDescriptor : ILspHandlerDescriptor, IDisposable, IEquatable<HandlerDescriptor>
    {
        private readonly Action _disposeAction;

        public HandlerDescriptor(string method, string key, IJsonRpcHandler handler, Type handlerType, Type @params, Type registrationType, Type capabilityType, Action disposeAction)
        {
            _disposeAction = disposeAction;
            Handler = handler;
            Method = method;
            Key = key;
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
            get
            {
                if (!HasRegistration) return null;
                if (_registration != null) return _registration;

                // TODO: Cache this
                var options = GetType()
                    .GetTypeInfo()
                    .GetMethod(nameof(GetRegistration), BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(RegistrationType)
                    .Invoke(this, new object[] { Handler });

                return _registration = new Registration()
                {
                    Id = Guid.NewGuid().ToString(),
                    Method = Method,
                    RegisterOptions = options
                };
            }
        }

        public void SetCapability(object instance)
        {
            if (instance is DynamicCapability dc)
            {
                AllowsDynamicRegistration = dc.DynamicRegistration;
            }

            // TODO: Cache this
            GetType()
                .GetTypeInfo()
                .GetMethod(nameof(SetCapability), BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(CapabilityType)
                .Invoke(this, new[] { Handler, instance });
        }

        public string Method { get; }
        public string Key { get; }
        public Type Params { get; }

        public bool IsDynamicCapability => typeof(DynamicCapability).GetTypeInfo().IsAssignableFrom(CapabilityType);
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

        public override bool Equals(object obj)
        {
            return Equals(obj as HandlerDescriptor);
        }

        public bool Equals(HandlerDescriptor other)
        {
            return other != null &&
                   EqualityComparer<Type>.Default.Equals(HandlerType, other.HandlerType) &&
                   Method == other.Method &&
                   Key == other.Key;
        }

        public override int GetHashCode()
        {
            var hashCode = -45133801;
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(HandlerType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Method);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Key);
            return hashCode;
        }

        public static bool operator ==(HandlerDescriptor descriptor1, HandlerDescriptor descriptor2)
        {
            return EqualityComparer<HandlerDescriptor>.Default.Equals(descriptor1, descriptor2);
        }

        public static bool operator !=(HandlerDescriptor descriptor1, HandlerDescriptor descriptor2)
        {
            return !(descriptor1 == descriptor2);
        }
    }
}
