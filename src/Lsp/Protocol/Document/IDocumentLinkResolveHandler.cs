using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("documentLink/resolve")]
    public interface IDocumentLinkResolveHandler : IRequestHandler<DocumentLink, DocumentLink> { }
}