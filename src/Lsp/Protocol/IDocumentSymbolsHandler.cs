using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentRegistrationOptions))]
    [Method("textDocument/documentSymbol")]
    public interface IDocumentSymbolsHandler : IRequestHandler<DocumentSymbolParams, SymbolInformationContainer> { }
}