using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class CodeActionExtensions
    {
        public static Task<CommandOrCodeActionContainer> CodeAction(this ILanguageClientDocument mediator, CodeActionParams @params, CancellationToken cancellationToken )
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
