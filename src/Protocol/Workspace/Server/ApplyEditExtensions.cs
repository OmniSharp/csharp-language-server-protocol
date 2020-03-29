using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public static class ApplyEditExtensions
    {
        public static Task<ApplyWorkspaceEditResponse> ApplyEdit(this ILanguageServerWorkspace mediator, ApplyWorkspaceEditParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest<ApplyWorkspaceEditParams, ApplyWorkspaceEditResponse>(WorkspaceNames.ApplyEdit, @params, cancellationToken);
        }
    }
}
