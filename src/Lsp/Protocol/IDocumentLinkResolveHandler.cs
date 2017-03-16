using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Method("documentLink/resolve")]
    public interface IDocumentLinkResolveHandler : IRequestHandler<DocumentLink, DocumentLink> { }
}