using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Method("workspace/applyEdit")]
    public interface IApplyEditHandler : IRequestHandler<ApplyWorkspaceEditParams, ApplyWorkspaceEditResponse>
    {
    }
}