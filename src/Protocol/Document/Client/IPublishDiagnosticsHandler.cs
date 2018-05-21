using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    using static DocumentNames;
    [Parallel, Method(PublishDiagnostics)]
    public interface IPublishDiagnosticsHandler : IJsonRpcNotificationHandler<PublishDiagnosticsParams> { }
}
