using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/completion")]
    public interface ICompletionHandler : IRegistrableRequestHandler<TextDocumentPositionParams, CompletionList, CompletionRegistrationOptions> { }
}