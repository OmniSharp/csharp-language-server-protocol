using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Serial, Method(ClientNames.UnregisterCapability)]
    public interface IUnregisterCapabilityHandler : IJsonRpcRequestHandler<UnregistrationParams> { }
}
