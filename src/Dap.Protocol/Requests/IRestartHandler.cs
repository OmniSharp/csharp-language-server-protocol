using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Restart, Direction.ClientToServer)]
    public interface IRestartHandler : IJsonRpcRequestHandler<RestartArguments, RestartResponse>
    {
    }

    public abstract class RestartHandler : IRestartHandler
    {
        public abstract Task<RestartResponse> Handle(RestartArguments request, CancellationToken cancellationToken);
    }

    public static class RestartExtensions
    {
        public static IDisposable OnRestart(this IDebugAdapterServerRegistry registry,
            Func<RestartArguments, CancellationToken, Task<RestartResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Restart, RequestHandler.For(handler));
        }

        public static IDisposable OnRestart(this IDebugAdapterServerRegistry registry,
            Func<RestartArguments, Task<RestartResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Restart, RequestHandler.For(handler));
        }

        public static Task<RestartResponse> RequestRestart(this IDebugAdapterClient mediator, RestartArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
