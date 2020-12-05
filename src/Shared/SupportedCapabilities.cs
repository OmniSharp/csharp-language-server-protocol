using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    internal class SupportedCapabilities : SupportedCapabilitiesBase, ISupportedCapabilities, ICapabilitiesProvider
    {
        public T GetCapability<T>() where T : ICapability?
        {
            if (TryGetCapability(typeof(T), out var value) && value is T c) return c;
            return default!;
        }

        public ICapability? GetCapability(Type type)
        {
            if (TryGetCapability(type, out var value) && value is ICapability c) return c;
            return default;
        }

        public bool AllowsDynamicRegistration(Type capabilityType)
        {
            if (capabilityType != null && TryGetCapability(capabilityType, out var capability))
            {
                if (capability is IDynamicCapability dc)
                    return dc.DynamicRegistration;
            }

            return false;
        }
    }

    internal class SupportedCapabilitiesBase
    {
        private readonly IDictionary<Type, object> _supports = new Dictionary<Type, object>();

        public void Initialize(ClientCapabilities clientCapabilities)
        {
            _clientCapabilities = clientCapabilities;
        }

        public void Add(IEnumerable<ISupports> supports)
        {
            foreach (var item in supports)
            {
                if (_supports.TryGetValue(item.ValueType, out _))
                    _supports.Remove(item.ValueType);
                _supports.Add(item.ValueType, item.Value!);
            }
        }

        public void Add(ICapability capability)
        {
            var valueType = capability.GetType();
            if (TryGetCapability(valueType, out _))
                _supports.Remove(valueType);
            _supports.Add(valueType, capability);
        }

        protected virtual bool TryGetCapability(Type capabilityType, [NotNullWhen(true)] out object? capability)
        {
            return _supports.TryGetValue(capabilityType, out capability);
        }

        public object? GetRegistrationOptions(ILspHandlerTypeDescriptor descriptor, IJsonRpcHandler handler)
        {
            return GetRegistrationOptions(descriptor.RegistrationType, descriptor.CapabilityType, handler);
        }

        public object? GetRegistrationOptions(ILspHandlerDescriptor descriptor, IJsonRpcHandler handler)
        {
            return GetRegistrationOptions(descriptor.RegistrationType, descriptor.CapabilityType, handler);
        }

        public object? GetRegistrationOptions(Type? registrationType, Type? capabilityType, IJsonRpcHandler handler)
        {
            // this method will play dual purpose, it will ensure that set capability has been called
            // even though that is not part of the method.
            if (!( registrationType is not null))
            {
                if (capabilityType is null || !TryGetCapability(capabilityType, out var capability)) return null;

                if (typeof(ICapability<>).MakeGenericType(capabilityType).IsInstanceOfType(handler))
                {
                    SetCapabilityInnerMethod
                       .MakeGenericMethod(capabilityType)
                       .Invoke(null, new[] { handler, capability, _clientCapabilities });
                }

                return null;
            }

            if (capabilityType is {}
             && typeof(IRegistration<,>).MakeGenericType(registrationType, capabilityType).IsInstanceOfType(handler))
            {
                if (TryGetCapability(capabilityType, out var capability))
                {
                    var result = SetRegistrationCapabilityInnerMethod
                                .MakeGenericMethod(registrationType, capabilityType)
                                .Invoke(null, new[] { handler, capability, _clientCapabilities });
                    return result;
                }
            }
            else if (!typeof(IRegistration<>).MakeGenericType(registrationType).IsInstanceOfType(handler))
            {
                var result = GetRegistrationOptionsInnerMethod
                            .MakeGenericMethod(registrationType)
                            .Invoke(null, new object[] { handler, _clientCapabilities });
                return result;
            }

            return null;
        }

        protected static readonly MethodInfo SetCapabilityInnerMethod = typeof(SupportedCapabilitiesBase)
                                                                       .GetTypeInfo()
                                                                       .GetMethod(nameof(SetCapabilityInner), BindingFlags.NonPublic | BindingFlags.Static)!;

        private static void SetCapabilityInner<T>(ICapability<T> capability, T instance, ClientCapabilities clientCapabilities) => capability.SetCapability(instance, clientCapabilities);

        private static readonly MethodInfo SetRegistrationCapabilityInnerMethod = typeof(SupportedCapabilitiesBase)
                                                                                 .GetTypeInfo()
                                                                                 .GetMethod(nameof(SetRegistrationCapabilityInner), BindingFlags.NonPublic | BindingFlags.Static)!;

        private static object SetRegistrationCapabilityInner<TR, TC>(IRegistration<TR, TC> capability, TC instance, ClientCapabilities clientCapabilities)
            where TR : class
            where TC : ICapability
        {
            var registrationCapabilityInner = capability.GetRegistrationOptions(instance, clientCapabilities);
            return registrationCapabilityInner;
        }

        private static readonly MethodInfo GetRegistrationOptionsInnerMethod = typeof(SupportedCapabilitiesBase)
                                                                              .GetTypeInfo()
                                                                              .GetMethod(nameof(GetRegistrationOptionsInner), BindingFlags.NonPublic | BindingFlags.Static)!;

        private ClientCapabilities _clientCapabilities;

        private static object GetRegistrationOptionsInner<TR>(IRegistration<TR> capability, ClientCapabilities clientCapabilities) where TR : class
        {
            var registrationOptionsInner = capability.GetRegistrationOptions(clientCapabilities);
            return registrationOptionsInner;
        }
    }
}
