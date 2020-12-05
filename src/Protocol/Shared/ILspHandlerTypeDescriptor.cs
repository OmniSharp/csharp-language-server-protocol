using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Shared
{
    public interface ILspHandlerTypeDescriptor : IHandlerTypeDescriptor
    {
        bool HasRegistration { get; }
        Type? RegistrationType { get; }
        string RegistrationMethod { get; }
        bool HasCapability { get; }
        Type? CapabilityType { get; }
        bool IsDynamicCapability { get; }
        Type? PartialItemsType { get; }
        Type? PartialItemType { get; }
        bool HasPartialItems { get; }
        bool HasPartialItem { get; }
    }
}
