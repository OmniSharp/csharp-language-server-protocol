using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Parallel, Method(WorkspaceNames.ApplyEdit)]
    public interface IApplyEditHandler : IJsonRpcRequestHandler<ApplyWorkspaceEditParams, ApplyWorkspaceEditResponse> { }
}
