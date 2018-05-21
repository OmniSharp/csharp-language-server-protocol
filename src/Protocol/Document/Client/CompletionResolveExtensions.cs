using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class CompletionResolveExtensions
    {
        public static Task<CompletionItem> CompletionResolve(this ILanguageClientDocument mediator, CompletionItem @params)
        {
            return mediator.SendRequest<CompletionItem, CompletionItem>(DocumentNames.CompletionResolve, @params);
        }
    }
}
