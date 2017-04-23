using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/completion")]
    public interface ICompletionHandler : IRequestHandler<TextDocumentPositionParams, CompletionList>, IRegistration<CompletionRegistrationOptions>, ICapability<CompletionCapability> { }
}