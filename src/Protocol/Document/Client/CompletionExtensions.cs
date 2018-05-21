using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class CompletionExtensions
    {
        public static Task<CompletionList> Completion(this ILanguageClientDocument mediator, CompletionParams @params)
        {
            return mediator.SendRequest<CompletionParams, CompletionList>(DocumentNames.Completion, @params);
        }
    }
}
