using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Shared
{
    [DebuggerDisplay("{" + nameof(Method) + "}")]
    class LspHandlerTypeDescriptor : HandlerTypeDescriptor, ILspHandlerTypeDescriptor
    {
        public LspHandlerTypeDescriptor(Type handlerType) : base(handlerType)
        {
            PartialItemsType = ParamsType.GetInterfaces().FirstOrDefault(z => z.IsGenericType && typeof(IPartialItems<>).IsAssignableFrom(z.GetGenericTypeDefinition()))
                ?.GetGenericArguments()[0];
            HasPartialItems = PartialItemsType != null;
            PartialItemType = ParamsType.GetInterfaces().FirstOrDefault(z => z.IsGenericType && typeof(IPartialItem<>).IsAssignableFrom(z.GetGenericTypeDefinition()))
                ?.GetGenericArguments()[0];
            HasPartialItem = PartialItemType != null;
            RegistrationType = HandlerTypeDescriptorHelper.UnwrapGenericType(typeof(IRegistration<>), handlerType);
            HasRegistration = RegistrationType != null && RegistrationType != typeof(object);
            if (!HasRegistration) RegistrationType = null;
            CapabilityType = HandlerTypeDescriptorHelper.UnwrapGenericType(typeof(ICapability<>), handlerType);
            HasCapability = CapabilityType != null;
            if (!HasCapability) CapabilityType = null;
            if (HasCapability)
                IsDynamicCapability = typeof(IDynamicCapability).GetTypeInfo().IsAssignableFrom(CapabilityType);
        }

        public Type PartialItemsType { get; }
        public Type PartialItemType { get; }
        public bool HasPartialItems { get; }
        public bool HasPartialItem { get; }
        public bool HasRegistration { get; }
        public Type RegistrationType { get; }
        public bool HasCapability { get; }
        public Type CapabilityType { get; }
        public bool IsDynamicCapability { get; }
        public override string ToString() => $"{Method}";
    }
}
