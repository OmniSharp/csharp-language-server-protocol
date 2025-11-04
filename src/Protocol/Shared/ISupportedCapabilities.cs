using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Shared
{
    public interface ISupportedCapabilities
    {
        bool AllowsDynamicRegistration(Type? capabilityType);
        object? GetRegistrationOptions(ILspHandlerTypeDescriptor handlerTypeDescriptor, IJsonRpcHandler handler);
        object? GetRegistrationOptions(ILspHandlerDescriptor handlerTypeDescriptor);
        void Add(IEnumerable<ISupports> supports);
        void Add(ICapability capability);
        void Initialize(ClientCapabilities clientCapabilities);
    }
}
