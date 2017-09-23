using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Abstractions;
using OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Client;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/definition")]
    public interface IDefinitionHandler : IRequestHandler<TextDocumentPositionParams, LocationOrLocations>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DefinitionCapability> { }
}