using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Abstractions;
using OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Client;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/references")]
    public interface IReferencesHandler : IRequestHandler<ReferenceParams, LocationContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<ReferencesCapability> { }
}