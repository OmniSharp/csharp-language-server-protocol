using System;
using System.Collections.Generic;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;

namespace OmniSharp.Extensions.LanguageServer.Shared
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
                if (_supports.TryGetValue(item.ValueType, out _))
                    _supports.Remove(item.ValueType);
                _supports.Add(item.ValueType, item.Value);
            }
        }

        public bool AllowsDynamicRegistration(Type capabilityType)
        {
            if (_supports.TryGetValue(capabilityType, out var capability))
            {
                if (capability is IDynamicCapability dc)
                    return dc.DynamicRegistration;
            }
            return false;
        }

        public void SetCapability(ILspHandlerDescriptor descriptor, IJsonRpcHandler handler)
        {
            if (!descriptor.HasCapability) return;
            if (descriptor.CapabilityType == null ||!handler.GetType().IsInstanceOfType(descriptor.CapabilityType)) return;

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
