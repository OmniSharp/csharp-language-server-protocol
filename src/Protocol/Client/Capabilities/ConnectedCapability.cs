using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    internal interface ConnectedCapability<out T> : ICapability
        where T : IJsonRpcHandler
    { }
}
