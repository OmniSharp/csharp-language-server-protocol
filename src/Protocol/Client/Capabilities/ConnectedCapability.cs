using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedTypeParameter
    internal interface ConnectedCapability<out T> : ICapability
        where T : IJsonRpcHandler
    {
    }
}
