using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServerProtocol;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    public static class ApplyEditExtensions
    {
        public static Task<ApplyWorkspaceEditResponse> ApplyEdit(this ILanguageServer mediator, ApplyWorkspaceEditParams @params)
        {
            return mediator.SendRequest<ApplyWorkspaceEditParams, ApplyWorkspaceEditResponse>("workspace/applyEdit", @params);
        }
    }
}