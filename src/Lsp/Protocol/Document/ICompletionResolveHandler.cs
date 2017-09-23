using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Method("completionItem/resolve")]
    public interface ICompletionResolveHandler : IRequestHandler<CompletionItem, CompletionItem> { }
}