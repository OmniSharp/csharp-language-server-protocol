using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(EventNames.Exited)]
    public interface IExitedHandler : IJsonRpcNotificationHandler<ExitedEvent> { }

    public abstract class ExitedHandler : IExitedHandler
    {
        public abstract Task<Unit> Handle(ExitedEvent request, CancellationToken cancellationToken);
    }
    public static class ExitedHandlerExtensions
    {
        public static IDisposable OnExited(this IDebugAdapterRegistry registry, Func<ExitedEvent, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ExitedHandler
        {
            private readonly Func<ExitedEvent, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<ExitedEvent, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(ExitedEvent request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }

}
