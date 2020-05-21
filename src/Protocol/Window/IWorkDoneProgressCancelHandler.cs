using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Window
{
    [Parallel, Method(WindowNames.WorkDoneProgressCancel, Direction.ClientToServer)]
    public interface IWorkDoneProgressCancelHandler : IJsonRpcNotificationHandler<WorkDoneProgressCancelParams> { }

    public abstract class WorkDoneProgressCancelHandler : IWorkDoneProgressCancelHandler
    {
        public abstract Task<Unit> Handle(WorkDoneProgressCancelParams request, CancellationToken cancellationToken);
    }

    public static class WorkDoneProgressCancelExtensions
    {
        public static IDisposable OnWorkDoneProgressCancel(
            this ILanguageServerRegistry registry,
            Action<WorkDoneProgressCancelParams, CancellationToken> handler)
        {
            return registry.AddHandler(WindowNames.WorkDoneProgressCancel, NotificationHandler.For(handler));
        }

        public static IDisposable OnWorkDoneProgressCancel(
            this ILanguageServerRegistry registry,
            Action<WorkDoneProgressCancelParams> handler)
        {
            return registry.AddHandler(WindowNames.WorkDoneProgressCancel, NotificationHandler.For(handler));
        }

        public static IDisposable OnWorkDoneProgressCancel(
            this ILanguageServerRegistry registry,
            Func<WorkDoneProgressCancelParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(WindowNames.WorkDoneProgressCancel, NotificationHandler.For(handler));
        }

        public static IDisposable OnWorkDoneProgressCancel(
            this ILanguageServerRegistry registry,
            Func<WorkDoneProgressCancelParams, Task> handler)
        {
            return registry.AddHandler(WindowNames.WorkDoneProgressCancel, NotificationHandler.For(handler));
        }

        public static void SendWorkDoneProgressCancel(this IWindowLanguageClient mediator, IWorkDoneProgressParams @params)
        {
            mediator.SendNotification(WindowNames.WorkDoneProgressCancel, new WorkDoneProgressCancelParams() {
                Token = @params.WorkDoneToken
            });
        }

        public static void SendWorkDoneProgressCancel(this IWindowLanguageClient mediator, ProgressToken token)
        {
            mediator.SendNotification(WindowNames.WorkDoneProgressCancel, new WorkDoneProgressCancelParams() {
                Token = token
            });
        }

        public static void SendWorkDoneProgressCancel(this IWindowLanguageClient mediator, WorkDoneProgressCancelParams token)
        {
            mediator.SendNotification(token);
        }
    }
}
