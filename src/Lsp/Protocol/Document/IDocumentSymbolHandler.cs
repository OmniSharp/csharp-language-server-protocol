using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/documentSymbol")]
    public interface IDocumentSymbolHandler : IRequestHandler<DocumentSymbolParams, SymbolInformationContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DocumentSymbolCapability> { }
}