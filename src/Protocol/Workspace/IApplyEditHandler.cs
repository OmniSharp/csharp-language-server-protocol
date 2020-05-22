using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Parallel, Method(WorkspaceNames.ApplyEdit, Direction.ServerToClient)]
    public interface IApplyWorkspaceEditHandler : IJsonRpcRequestHandler<ApplyWorkspaceEditParams, ApplyWorkspaceEditResponse> { }

    public abstract class ApplyWorkspaceEditHandler : IApplyWorkspaceEditHandler
    {
        public abstract Task<ApplyWorkspaceEditResponse> Handle(ApplyWorkspaceEditParams request, CancellationToken cancellationToken);
    }

    public static class ApplyWorkspaceEditExtensions
    {
        public static IDisposable OnApplyWorkspaceEdit(
            this ILanguageClientRegistry registry,
            Func<ApplyWorkspaceEditParams, CancellationToken, Task<ApplyWorkspaceEditResponse>>
                handler)
        {
            return registry.AddHandler(WorkspaceNames.ApplyEdit, RequestHandler.For(handler));
        }

        public static IDisposable OnApplyWorkspaceEdit(
            this ILanguageClientRegistry registry,
            Func<ApplyWorkspaceEditParams, Task<ApplyWorkspaceEditResponse>> handler)
        {
            return registry.AddHandler(WorkspaceNames.ApplyEdit, RequestHandler.For(handler));
        }

        public static Task<ApplyWorkspaceEditResponse> ApplyWorkspaceEdit(this IWorkspaceLanguageServer mediator, ApplyWorkspaceEditParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
