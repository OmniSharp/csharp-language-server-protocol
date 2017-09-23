using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class ApplyEditExtensions
    {
        public static Task<ApplyWorkspaceEditResponse> ApplyEdit(this ILanguageServer mediator, ApplyWorkspaceEditParams @params)
        {
            return mediator.SendRequest<ApplyWorkspaceEditParams, ApplyWorkspaceEditResponse>("workspace/applyEdit", @params);
        }
    }
}