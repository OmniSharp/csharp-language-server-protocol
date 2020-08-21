using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    internal interface ConnectedCapability<out T> : ICapability
        where T : IJsonRpcHandler
    {
    }
}
