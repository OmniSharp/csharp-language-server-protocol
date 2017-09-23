using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Abstractions;
using OmniSharp.Extensions.LanguageServer.Capabilities.Client;
using OmniSharp.Extensions.LanguageServer.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Method("textDocument/references")]
    public interface IReferencesHandler : IRequestHandler<ReferenceParams, LocationContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<ReferencesCapability> { }
}