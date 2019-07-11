using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Serial, Method(GeneralNames.Exit)]
    public interface IExitHandler : IJsonRpcNotificationHandler { }

    public abstract class ExitHandler : IExitHandler
    {
        public abstract Task<Unit> Handle(EmptyRequest request, CancellationToken cancellationToken);
    }

    public static class ExitHandlerExtensions
    {
        public static IDisposable OnExit(this ILanguageServerRegistry registry, Action handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ExitHandler
        {
            private readonly Action _handler;

            public DelegatingHandler(Action handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(EmptyRequest request, CancellationToken cancellationToken)
            {
                _handler.Invoke();
                return Unit.Task;
            }
        }
    }
}
