using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.General
{
    [Serial, Method(GeneralNames.Exit, Direction.ClientToServer)]
    public interface IExitHandler : IJsonRpcNotificationHandler<ExitParams>
    {
    }

    public abstract class ExitHandler : IExitHandler
    {
        public virtual async Task<Unit> Handle(ExitParams request, CancellationToken cancellationToken)
        {
            await Handle(cancellationToken);
            return Unit.Value;
        }

        protected abstract Task Handle(CancellationToken cancellationToken);
    }

    public static class ExitExtensions
    {
        public static IDisposable OnExit(this ILanguageServerRegistry registry, Action<ExitParams> handler)
        {
            return registry.AddHandler(GeneralNames.Exit,
                NotificationHandler.For(handler));
        }

        public static IDisposable OnExit(this ILanguageServerRegistry registry, Func<ExitParams, Task> handler)
        {
            return registry.AddHandler(GeneralNames.Exit,
                NotificationHandler.For(handler));
        }

        public static IDisposable OnExit(this ILanguageServerRegistry registry, Action<ExitParams, CancellationToken> handler)
        {
            return registry.AddHandler(GeneralNames.Exit,
                NotificationHandler.For(handler));
        }

        public static IDisposable OnExit(this ILanguageServerRegistry registry, Func<ExitParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(GeneralNames.Exit,
                NotificationHandler.For(handler));
        }

        public static void SendExit(this ILanguageClient mediator)
        {
            mediator.SendNotification(ExitParams.Instance);
        }

        public static void SendExit(this ILanguageClient mediator, ExitParams @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
