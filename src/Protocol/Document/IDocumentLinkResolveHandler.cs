using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static DocumentNames;
    public static partial class DocumentNames
    {
        public const string DocumentLinkResolve = "documentLink/resolve";
    }

    [Parallel, Method(DocumentLinkResolve)]
    public interface IDocumentLinkResolveHandler : IJsonRpcRequestHandler<DocumentLink, DocumentLink> { }
}
