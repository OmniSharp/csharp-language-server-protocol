using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/completion")]
    public interface ICompletionHandler : IRequestHandler<TextDocumentPositionParams, CompletionList>, IRegistration<CompletionRegistrationOptions> { }
}