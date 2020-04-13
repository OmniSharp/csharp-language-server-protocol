using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(WorkspaceNames.WorkspaceFolders)]
    public class WorkspaceFolderParams : IRequest<Container<WorkspaceFolder>>
    {

    }
}
