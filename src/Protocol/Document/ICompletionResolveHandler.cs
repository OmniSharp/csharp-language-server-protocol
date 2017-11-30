using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Serial, Method("completionItem/resolve")]
    public interface ICompletionResolveHandler : IRequestHandler<CompletionItem, CompletionItem> { }
}
