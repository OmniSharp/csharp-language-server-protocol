using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Abstractions;
using OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Client;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/didChange")]
    public interface IDidChangeTextDocumentHandler : INotificationHandler<DidChangeTextDocumentParams>, IRegistration<TextDocumentChangeRegistrationOptions>, ICapability<SynchronizationCapability> { }
}
