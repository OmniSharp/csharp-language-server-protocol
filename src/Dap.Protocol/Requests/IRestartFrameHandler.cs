using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.RestartFrame, Direction.ClientToServer)]
    public interface IRestartFrameHandler : IJsonRpcRequestHandler<RestartFrameArguments, RestartFrameResponse>
    {
    }

    public abstract class RestartFrameHandler : IRestartFrameHandler
    {
        public abstract Task<RestartFrameResponse> Handle(RestartFrameArguments request,
            CancellationToken cancellationToken);
    }

    public static class RestartFrameExtensions
    {
        public static IDisposable OnRestartFrame(this IDebugAdapterServerRegistry registry,
            Func<RestartFrameArguments, CancellationToken, Task<RestartFrameResponse>> handler)
        {
            return registry.AddHandler(RequestNames.RestartFrame, RequestHandler.For(handler));
        }

        public static IDisposable OnRestartFrame(this IDebugAdapterServerRegistry registry,
            Func<RestartFrameArguments, Task<RestartFrameResponse>> handler)
        {
            return registry.AddHandler(RequestNames.RestartFrame, RequestHandler.For(handler));
        }

        public static Task<RestartFrameResponse> RequestRestartFrame(this IDebugAdapterClient mediator, RestartFrameArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
