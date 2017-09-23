using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("documentLink/resolve")]
    public interface IDocumentLinkResolveHandler : IRequestHandler<DocumentLink, DocumentLink> { }
}