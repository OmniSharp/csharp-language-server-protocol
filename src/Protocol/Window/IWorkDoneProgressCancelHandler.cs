using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Window
{
    [Parallel]
    [Method(WindowNames.WorkDoneProgressCancel, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(IWindowLanguageClient), typeof(ILanguageClient))]
    public interface IWorkDoneProgressCancelHandler : IJsonRpcNotificationHandler<WorkDoneProgressCancelParams>
    {
    }

    public abstract class WorkDoneProgressCancelHandler : IWorkDoneProgressCancelHandler
    {
        public abstract Task<Unit> Handle(WorkDoneProgressCancelParams request, CancellationToken cancellationToken);
    }

    public static partial class WorkDoneProgressCancelExtensions
    {
        public static void SendWorkDoneProgressCancel(this IWindowLanguageClient mediator, IWorkDoneProgressParams @params) =>
            mediator.SendNotification(
                WindowNames.WorkDoneProgressCancel, new WorkDoneProgressCancelParams {
                    Token = @params.WorkDoneToken
                }
            );

        public static void SendWorkDoneProgressCancel(this IWindowLanguageClient mediator, ProgressToken token) =>
            mediator.SendNotification(
                WindowNames.WorkDoneProgressCancel, new WorkDoneProgressCancelParams {
                    Token = token
                }
            );
    }
}
