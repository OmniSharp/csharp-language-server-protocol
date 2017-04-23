using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/documentHighlight")]
    public interface IDocumentHighlightHandler : IRegistrableRequestHandler<TextDocumentPositionParams, DocumentHighlightContainer, TextDocumentRegistrationOptions> { }
}