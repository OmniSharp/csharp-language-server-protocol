using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Pause, Direction.ClientToServer)]
    public interface IPauseHandler : IJsonRpcRequestHandler<PauseArguments, PauseResponse>
    {
    }

    public abstract class PauseHandler : IPauseHandler
    {
        public abstract Task<PauseResponse> Handle(PauseArguments request, CancellationToken cancellationToken);
    }

    public static class PauseExtensions
    {
        public static IDebugAdapterServerRegistry OnPause(this IDebugAdapterServerRegistry registry,
            Func<PauseArguments, CancellationToken, Task<PauseResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Pause, RequestHandler.For(handler));
        }

        public static IDebugAdapterServerRegistry OnPause(this IDebugAdapterServerRegistry registry,
            Func<PauseArguments, Task<PauseResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Pause, RequestHandler.For(handler));
        }

        public static Task<PauseResponse> RequestPause(this IDebugAdapterClient mediator, PauseArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
