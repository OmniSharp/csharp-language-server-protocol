using System.Threading.Tasks;
using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    public static partial class ResponseHandlerExtensions
    {
        public static Task<ApplyWorkspaceEditResponse> ApplyEdit(this IClient mediator, ApplyWorkspaceEditParams @params)
        {
            return mediator.SendRequest<ApplyWorkspaceEditParams, ApplyWorkspaceEditResponse>("workspace/applyEdit", @params);
        }
    }
}