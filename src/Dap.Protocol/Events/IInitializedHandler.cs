using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(EventNames.Initialized)]
    public interface IInitializedHandler : IJsonRpcNotificationHandler<InitializedEvent> { }

    public abstract class InitializedHandler : IInitializedHandler
    {
        public abstract Task<Unit> Handle(InitializedEvent request, CancellationToken cancellationToken);
    }
    public static class InitializedHandlerExtensions
    {
        public static IDisposable OnInitializedTextDocument(this IDebugAdapterRegistry registry, Func<InitializedEvent, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : InitializedHandler
        {
            private readonly Func<InitializedEvent, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<InitializedEvent, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(InitializedEvent request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
