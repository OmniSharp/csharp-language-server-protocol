using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Capabilities.Client
{
    internal interface ConnectedCapability<out T>
        where T : IJsonRpcHandler
    { }
}
