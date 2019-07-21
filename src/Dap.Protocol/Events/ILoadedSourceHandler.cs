using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(EventNames.LoadedSource)]
    public interface ILoadedSourceHandler : IJsonRpcNotificationHandler<LoadedSourceEvent> { }

    public abstract class LoadedSourceHandler : ILoadedSourceHandler
    {
        public abstract Task<Unit> Handle(LoadedSourceEvent request, CancellationToken cancellationToken);
    }
    public static class LoadedSourceHandlerExtensions
    {
        public static IDisposable OnLoadedSource(this IDebugAdapterRegistry registry, Func<LoadedSourceEvent, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : LoadedSourceHandler
        {
            private readonly Func<LoadedSourceEvent, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<LoadedSourceEvent, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(LoadedSourceEvent request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
