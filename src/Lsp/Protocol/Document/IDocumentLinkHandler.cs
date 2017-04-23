using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/documentLink")]
    public interface IDocumentLinkHandler : IRegistrableRequestHandler<DocumentLinkParams, DocumentLink, DocumentLinkRegistrationOptions> { }
}