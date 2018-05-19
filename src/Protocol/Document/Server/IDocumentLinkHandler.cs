using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    using static DocumentNames;
    [Parallel, Method(DocumentLink)]
    public interface IDocumentLinkHandler : IJsonRpcRequestHandler<DocumentLinkParams, DocumentLinkContainer>, IRegistration<DocumentLinkRegistrationOptions>, ICapability<DocumentLinkCapability> { }
}
