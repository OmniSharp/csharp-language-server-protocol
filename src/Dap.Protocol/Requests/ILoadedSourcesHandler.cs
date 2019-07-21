using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.LoadedSources)]
    public interface ILoadedSourcesHandler : IJsonRpcRequestHandler<LoadedSourcesArguments, LoadedSourcesResponse> { }

    public abstract class LoadedSourcesHandler : ILoadedSourcesHandler
    {
        public abstract Task<LoadedSourcesResponse> Handle(LoadedSourcesArguments request, CancellationToken cancellationToken);
    }

    public static class LoadedSourcesHandlerExtensions
    {
        public static IDisposable OnLoadedSources(this IDebugAdapterRegistry registry, Func<LoadedSourcesArguments, CancellationToken, Task<LoadedSourcesResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : LoadedSourcesHandler
        {
            private readonly Func<LoadedSourcesArguments, CancellationToken, Task<LoadedSourcesResponse>> _handler;

            public DelegatingHandler(Func<LoadedSourcesArguments, CancellationToken, Task<LoadedSourcesResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<LoadedSourcesResponse> Handle(LoadedSourcesArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
