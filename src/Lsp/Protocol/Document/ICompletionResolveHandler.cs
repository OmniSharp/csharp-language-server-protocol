using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("completionItem/resolve")]
    public interface ICompletionResolveHandler : IRequestHandler<CompletionItem, CompletionItem> { }
}