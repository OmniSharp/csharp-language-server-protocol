using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/rename")]
    public interface IRenameHandler : IRequestHandler<RenameParams, WorkspaceEdit>, IRegistration<TextDocumentRegistrationOptions>, ICapability<RenameCapability> { }
}