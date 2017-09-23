using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Method("documentLink/resolve")]
    public interface IDocumentLinkResolveHandler : IRequestHandler<DocumentLink, DocumentLink> { }
}