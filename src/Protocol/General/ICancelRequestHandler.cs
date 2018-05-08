using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static GeneralNames;
    [Parallel, Method(CancelRequest)]
    public interface ICancelRequestHandler : IJsonRpcNotificationHandler<CancelParams> { }
}
