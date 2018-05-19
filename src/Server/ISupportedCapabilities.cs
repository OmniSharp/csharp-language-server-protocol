using System;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public interface ISupportedCapabilities
    {
        bool AllowsDynamicRegistration(ILspHandlerDescriptor descriptor);
        bool AllowsDynamicRegistration(Type capabilityType);
    }
}
