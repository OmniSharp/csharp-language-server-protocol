using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Parallel, Method(WorkspaceNames.ApplyEdit)]
    public interface IWorkspaceFoldersHandler : IJsonRpcRequestHandler<WorkspaceFolderParams, Container<WorkspaceFolder>> { }
}
