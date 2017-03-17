using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(DocumentLinkRegistrationOptions))]
    [Method("textDocument/documentLink")]
    public interface IDocumentLinkHandler : IRequestHandler<DocumentLinkParams, DocumentLink> { }
}