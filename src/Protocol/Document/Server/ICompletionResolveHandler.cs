using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    using static DocumentNames;
    [Serial, Method(CompletionResolve)]
    public interface ICompletionResolveHandler : ICanBeResolvedHandler<CompletionItem> { }
}
