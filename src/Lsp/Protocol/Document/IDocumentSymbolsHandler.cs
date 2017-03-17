using JsonRPC;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentRegistrationOptions))]
    [Method("textDocument/documentSymbol")]
    public interface IDocumentSymbolsHandler : IRequestHandler<DocumentSymbolParams, SymbolInformationContainer> { }
}