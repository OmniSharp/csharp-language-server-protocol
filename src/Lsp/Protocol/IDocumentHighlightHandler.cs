using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentRegistrationOptions))]
    [Method("textDocument/documentHighlight")]
    public interface IDocumentHighlightHandler : IRequestHandler<TextDocumentPositionParams, DocumentHighlightContainer> { }
}