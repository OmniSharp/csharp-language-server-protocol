using System;
using System.Collections.Generic;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    class SupportedCapabilities : ISupportedCapabilities
    {
        private static readonly MethodInfo SetCapabilityInnerMethod = typeof(SupportedCapabilities)
            .GetTypeInfo()
            .GetMethod(nameof(SetCapabilityInner), BindingFlags.NonPublic | BindingFlags.Static);

        private readonly IDictionary<Type, object> _supports = new Dictionary<Type, object>();
        public SupportedCapabilities()
        {
        }

        public void Add(IEnumerable<ISupports> supports)
        {
            foreach (var item in supports)
            {
                if (!_supports.TryGetValue(item.ValueType, out _))
                    _supports.Add(item.ValueType, item.Value);
            }
        }

        public bool AllowsDynamicRegistration(ILspHandlerDescriptor descriptor)
        {
            if (descriptor.HasCapability && _supports.TryGetValue(descriptor.CapabilityType, out var capability))
            {
                if (capability is DynamicCapability dc)
                    return dc.DynamicRegistration;
            }
            return false;
        }

        public bool AllowsDynamicRegistration(Type capabilityType)
        {
            if (_supports.TryGetValue(capabilityType, out var capability))
            {
                if (capability is DynamicCapability dc)
                    return dc.DynamicRegistration;
            }
            return false;
        }

        public void SetCapability(ILspHandlerDescriptor descriptor, IJsonRpcHandler handler)
        {
            if (!descriptor.HasCapability) return;

            if (_supports.TryGetValue(descriptor.CapabilityType, out var capability))
            {
                SetCapabilityInnerMethod
                    .MakeGenericMethod(descriptor.CapabilityType)
                    .Invoke(null, new[] { handler, capability });
            }
        }

        private static void SetCapabilityInner<T>(ICapability<T> capability, T instance)
        {
            capability.SetCapability(instance);
        }
    }
}
