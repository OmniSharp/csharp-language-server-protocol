using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Abstractions;
using OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Client;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/didOpen")]
    public interface IDidOpenTextDocumentHandler : INotificationHandler<DidOpenTextDocumentParams>, IRegistration<TextDocumentRegistrationOptions>, ICapability<SynchronizationCapability> { }
}