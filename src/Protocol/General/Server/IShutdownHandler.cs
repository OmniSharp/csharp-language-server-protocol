using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    using static GeneralNames;
    [Serial, Method(Shutdown)]
    public interface IShutdownHandler : IJsonRpcRequestHandler<EmptyRequest> { }
}
