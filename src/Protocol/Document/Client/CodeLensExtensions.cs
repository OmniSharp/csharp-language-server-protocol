using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class CodeLensExtensions
    {
        public static Task<CodeLensContainer> CodeLens(this ILanguageClientDocument mediator, CodeLensParams @params)
        {
            return mediator.SendRequest<CodeLensParams, CodeLensContainer>(DocumentNames.CodeLens, @params);
        }
    }
}
