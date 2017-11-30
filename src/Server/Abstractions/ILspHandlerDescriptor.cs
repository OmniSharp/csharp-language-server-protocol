using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Models;

namespace OmniSharp.Extensions.LanguageServer.Abstractions
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
    }
}
