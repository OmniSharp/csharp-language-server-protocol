using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/definition")]
    public interface IDefinitionHandler : IRequestHandler<TextDocumentPositionParams, LocationOrLocations>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DefinitionCapability> { }
}