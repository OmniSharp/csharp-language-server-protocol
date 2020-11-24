using System;
using System.Collections.Generic;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    internal class SupportedCapabilities : ISupportedCapabilities, ICapabilitiesProvider
    {
        private readonly IDictionary<Type, object> _supports = new Dictionary<Type, object>();

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
            if (_supports.TryGetValue(valueType, out _))
                _supports.Remove(valueType);
            _supports.Add(valueType, capability);
        }

        public T GetCapability<T>() where T : ICapability?
        {
            if (_supports.TryGetValue(typeof(T), out var value) && value is T c) return c;
            return default!;
        }

        public ICapability? GetCapability(Type type)
        {
            if (_supports.TryGetValue(type, out var value) && value is ICapability c) return c;
            return default;
        }

        public bool AllowsDynamicRegistration(Type capabilityType)
        {
            if (capabilityType != null && _supports.TryGetValue(capabilityType, out var capability))
            {
                if (capability is IDynamicCapability dc)
                    return dc.DynamicRegistration;
            }

            return false;
        }

        public void SetCapability(ILspHandlerTypeDescriptor descriptor, IJsonRpcHandler handler)
        {
            if (!( descriptor is { HasCapability: true } and { CapabilityType: not null } )) return;

            if ( descriptor is { HasRegistration: true } and { RegistrationType: not null }
             && typeof(IRegistration<,>).MakeGenericType(descriptor.RegistrationType, descriptor.CapabilityType).IsInstanceOfType(handler))
            {
                if (_supports.TryGetValue(descriptor.CapabilityType, out var capability))
                {
                    SetRegistrationCapabilityInnerMethod
                       .MakeGenericMethod(descriptor.RegistrationType, descriptor.CapabilityType)
                       .Invoke(null, new[] { handler, capability });
                }
            }
            else if (!typeof(ICapability<>).MakeGenericType(descriptor.CapabilityType).IsInstanceOfType(handler))
            {
                if (_supports.TryGetValue(descriptor.CapabilityType, out var capability))
                {
                    SetCapabilityInnerMethod
                                .MakeGenericMethod(descriptor.CapabilityType)
                                .Invoke(null, new[] { handler, capability });
                }
            }
        }

        public object? GetRegistrationOptions(ILspHandlerTypeDescriptor descriptor, IJsonRpcHandler handler)
        {
            if (!( descriptor is { HasRegistration: true } and { RegistrationType: not null } )) return null;

            if ( descriptor is { HasCapability: true } and { CapabilityType: not null }
             && typeof(IRegistration<,>).MakeGenericType(descriptor.RegistrationType, descriptor.CapabilityType).IsInstanceOfType(handler))
            {
                if (_supports.TryGetValue(descriptor.CapabilityType, out var capability))
                {
                    SetRegistrationCapabilityInnerMethod
                       .MakeGenericMethod(descriptor.RegistrationType, descriptor.CapabilityType)
                       .Invoke(null, new[] { handler, capability });
                }
            }
            else if (!typeof(IRegistration<>).MakeGenericType(descriptor.RegistrationType).IsInstanceOfType(handler))
            {
                GetRegistrationOptionsInnerMethod
                   .MakeGenericMethod(descriptor.RegistrationType)
                   .Invoke(null, new[] { handler });
            }

            return null;
        }

        private static readonly MethodInfo SetCapabilityInnerMethod = typeof(SupportedCapabilities)
                                                                     .GetTypeInfo()
                                                                     .GetMethod(nameof(SetCapabilityInner), BindingFlags.NonPublic | BindingFlags.Static)!;

        private static void SetCapabilityInner<T>(ICapability<T> capability, T instance) => capability.SetCapability(instance);

        private static readonly MethodInfo SetRegistrationCapabilityInnerMethod = typeof(SupportedCapabilities)
                                                                                 .GetTypeInfo()
                                                                                 .GetMethod(nameof(SetRegistrationCapabilityInner), BindingFlags.NonPublic | BindingFlags.Static)!;

        private static void SetRegistrationCapabilityInner<TR, TC>(IRegistration<TR, TC> capability, TC instance)
            where TR : class
            where TC : ICapability
            => capability.GetRegistrationOptions(instance);

        private static readonly MethodInfo GetRegistrationOptionsInnerMethod = typeof(SupportedCapabilities)
                                                                                 .GetTypeInfo()
                                                                                 .GetMethod(nameof(GetRegistrationOptionsInner), BindingFlags.NonPublic | BindingFlags.Static)!;

        private static void GetRegistrationOptionsInner<TR>(IRegistration<TR> capability) where TR : class => capability.GetRegistrationOptions();
    }
}
