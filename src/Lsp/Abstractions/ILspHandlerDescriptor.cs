using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

namespace OmniSharp.Extensions.LanguageServerProtocol.Abstractions
{
    public interface ILspHandlerDescriptor : IHandlerInstance
    {
        bool HasRegistration { get; }
        Type RegistrationType { get; }
        Registration Registration { get; }

        bool HasCapability { get; }
        Type CapabilityType { get; }
        void SetCapability(object instance);
        bool IsDynamicCapability { get; }
        bool AllowsDynamicRegistration { get; }
    }
}