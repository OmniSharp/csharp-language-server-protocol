using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class CodeActionExtensions
    {
        public static Task<CommandOrCodeActionContainer> CodeAction(this ILanguageClientDocument mediator, CodeActionParams @params)
        {
            return mediator.SendRequest<CodeActionParams, CommandOrCodeActionContainer>(DocumentNames.CodeAction, @params);
        }
    }
}
