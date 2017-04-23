using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/documentSymbol")]
    public interface IDocumentSymbolsHandler : IRequestHandler<DocumentSymbolParams, SymbolInformationContainer>, IRegistration<TextDocumentRegistrationOptions> { }
}