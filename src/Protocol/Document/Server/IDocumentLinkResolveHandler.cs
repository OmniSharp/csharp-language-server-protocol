using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    using static DocumentNames;
    [Parallel, Method(DocumentLinkResolve)]
    public interface IDocumentLinkResolveHandler : ICanBeResolvedHandler<DocumentLink> { }
}
