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
        void SetCapability(object instance);
        bool IsDynamicCapability { get; }
        bool AllowsDynamicRegistration { get; }
        Type CanBeResolvedHandlerType { get; }
    }
}
