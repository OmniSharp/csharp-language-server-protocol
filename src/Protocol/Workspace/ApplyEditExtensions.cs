using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static partial class WorkspaceNames
    {
        public const string ApplyEdit = "workspace/applyEdit";
    }

    public static class ApplyEditExtensions
    {
        public static Task<ApplyWorkspaceEditResponse> ApplyEdit(this IResponseRouter mediator, ApplyWorkspaceEditParams @params)
        {
            return mediator.SendRequest<ApplyWorkspaceEditParams, ApplyWorkspaceEditResponse>(WorkspaceNames.ApplyEdit, @params);
        }
    }
}
