using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.LoadedSources, Direction.ClientToServer)]
    public interface ILoadedSourcesHandler : IJsonRpcRequestHandler<LoadedSourcesArguments, LoadedSourcesResponse>
    {
    }

    public abstract class LoadedSourcesHandler : ILoadedSourcesHandler
    {
        public abstract Task<LoadedSourcesResponse> Handle(LoadedSourcesArguments request,
            CancellationToken cancellationToken);
    }

    public static class LoadedSourcesExtensions
    {
        public static IDebugAdapterServerRegistry OnLoadedSources(this IDebugAdapterServerRegistry registry,
            Func<LoadedSourcesArguments, CancellationToken, Task<LoadedSourcesResponse>> handler)
        {
            return registry.AddHandler(RequestNames.LoadedSources, RequestHandler.For(handler));
        }

        public static IDebugAdapterServerRegistry OnLoadedSources(this IDebugAdapterServerRegistry registry,
            Func<LoadedSourcesArguments, Task<LoadedSourcesResponse>> handler)
        {
            return registry.AddHandler(RequestNames.LoadedSources, RequestHandler.For(handler));
        }

        public static Task<LoadedSourcesResponse> RequestLoadedSources(this IDebugAdapterClient mediator, LoadedSourcesArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
