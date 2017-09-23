using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Abstractions;
using OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Client;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/rename")]
    public interface IRenameHandler : IRequestHandler<RenameParams, WorkspaceEdit>, IRegistration<TextDocumentRegistrationOptions>, ICapability<RenameCapability> { }
}