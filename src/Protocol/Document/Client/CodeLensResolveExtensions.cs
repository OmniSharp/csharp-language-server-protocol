using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class CodeLensResolveExtensions
    {
        public static Task<CodeLens> CodeLensResolve(this ILanguageClientDocument mediator, CodeLens @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest<CodeLens, CodeLens>(DocumentNames.CodeLensResolve, @params, cancellationToken);
        }
    }
}
