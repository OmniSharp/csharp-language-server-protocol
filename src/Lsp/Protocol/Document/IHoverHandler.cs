using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentRegistrationOptions))]
    [Method("textDocument/hover")]
    public interface IHoverHandler : IRequestHandler<TextDocumentPositionParams, Hover> { }
}