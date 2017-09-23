using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Abstractions;
using OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Client;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/documentLink")]
    public interface IDocumentLinkHandler : IRequestHandler<DocumentLinkParams, DocumentLink>, IRegistration<DocumentLinkRegistrationOptions>, ICapability<DocumentLinkCapability> { }
}