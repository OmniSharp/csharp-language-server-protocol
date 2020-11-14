using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Window
{
    [Parallel]
    [Method(WindowNames.ShowDocument, Direction.ServerToClient)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(IWindowLanguageServer), typeof(ILanguageServer))]
    public interface IShowDocumentRequestHandler : IJsonRpcRequestHandler<ShowDocumentParams, ShowDocumentResult>
    {
    }

    public abstract class ShowDocumentRequestHandler : IShowDocumentRequestHandler
    {
        public abstract Task<ShowDocumentResult> Handle(ShowDocumentParams request, CancellationToken cancellationToken);
    }
}
