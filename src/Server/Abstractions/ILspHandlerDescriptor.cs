using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Server.Abstractions
{
    public interface ILspHandlerDescriptor : IHandlerDescriptor
    {
        Guid Id { get; }
        bool HasRegistration { get; }
        Type RegistrationType { get; }
        object RegistrationOptions { get; }
        bool AllowsDynamicRegistration { get; }

        bool HasCapability { get; }
        Type CapabilityType { get; }
        bool IsDynamicCapability { get; }
        Type CanBeResolvedHandlerType { get; }
    }
}
