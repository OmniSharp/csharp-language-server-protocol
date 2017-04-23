using Lsp.Capabilities.Server;

namespace Lsp.Protocol
{
    public interface ITextDocumentSyncHandler : IDidChangeTextDocumentHandler, IDidOpenTextDocumentHandler, IDidCloseTextDocumentHandler, IDidSaveTextDocumentHandler
    {
        TextDocumentSyncOptions Options { get; }
    }
}