using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(WorkspaceNames.WorkspaceFolders, Direction.ServerToClient)]
    public class WorkspaceFolderParams : IRequest<Container<WorkspaceFolder>>
    {
        public WorkspaceFolderParams()
        {

        }

        public static WorkspaceFolderParams Instance = new WorkspaceFolderParams();
    }
}
