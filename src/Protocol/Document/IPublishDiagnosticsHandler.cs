using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{

    [Parallel, Method(TextDocumentNames.PublishDiagnostics, Direction.ServerToClient)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageServer), typeof(ILanguageServer))]
    public interface IPublishDiagnosticsHandler : IJsonRpcNotificationHandler<PublishDiagnosticsParams> { }

    public abstract class PublishDiagnosticsHandler : IPublishDiagnosticsHandler
    {
        public abstract Task<Unit> Handle(PublishDiagnosticsParams request, CancellationToken cancellationToken);
    }
}
