using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Method("completionItem/resolve")]
    public interface ICompletionResolveHandler : IRequestHandler<CompletionItem, CompletionItem> { }
}