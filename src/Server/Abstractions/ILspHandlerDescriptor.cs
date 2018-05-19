using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Server.Abstractions
{
    public interface ILspHandlerDescriptor : IHandlerDescriptor
    {
        bool HasRegistration { get; }
        Type RegistrationType { get; }
        Registration Registration { get; }

        bool HasCapability { get; }
        Type CapabilityType { get; }
        bool IsDynamicCapability { get; }
        Type CanBeResolvedHandlerType { get; }
    }
}
