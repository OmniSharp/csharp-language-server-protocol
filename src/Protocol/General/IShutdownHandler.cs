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
    [Serial, Method(GeneralNames.Shutdown, Direction.ClientToServer)]
    public interface IShutdownHandler : IJsonRpcRequestHandler<ShutdownParams>
    {
    }

    public abstract class ShutdownHandler : IShutdownHandler
    {
        public virtual async Task<Unit> Handle(ShutdownParams request, CancellationToken cancellationToken)
        {
            await Handle(cancellationToken);
            return Unit.Value;
        }

        protected abstract Task Handle(CancellationToken cancellationToken);
    }

    public static class ShutdownExtensions
    {
        public static IDisposable OnShutdown(
            this ILanguageServerRegistry registry,
            Func<ShutdownParams, CancellationToken, Task>
                handler)
        {
            return registry.AddHandler(GeneralNames.Shutdown,
                RequestHandler.For<ShutdownParams, Unit>(async (_, ct) => {
                    await handler(_, ct);
                    return Unit.Value;
                }));
        }

        public static IDisposable OnShutdown(
            this ILanguageServerRegistry registry,
            Func<ShutdownParams, Task>
                handler)
        {
            return registry.AddHandler(GeneralNames.Shutdown,
                RequestHandler.For<ShutdownParams, Unit>(async (_, ct) => {
                    await handler(_);
                    return Unit.Value;
                }));
        }

        public static Task RequestShutdown(this ILanguageClient mediator, ShutdownParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }

        public static Task RequestShutdown(this ILanguageClient mediator, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(ShutdownParams.Instance, cancellationToken);
        }
    }
}
