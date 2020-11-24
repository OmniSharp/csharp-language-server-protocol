using System;
using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Shared
{
    public interface ISupportedCapabilities
    {
        bool AllowsDynamicRegistration(Type capabilityType);
        void SetCapability(ILspHandlerTypeDescriptor descriptor, IJsonRpcHandler handler);
        object? GetRegistrationOptions(ILspHandlerTypeDescriptor handlerTypeDescriptor, IJsonRpcHandler handler);
        void Add(IEnumerable<ISupports> supports);
        void Add(ICapability capability);
    }
}
