using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.ReverseContinue, Direction.ClientToServer)]
    public interface IReverseContinueHandler : IJsonRpcRequestHandler<ReverseContinueArguments, ReverseContinueResponse>
    {
    }

    public abstract class ReverseContinueHandler : IReverseContinueHandler
    {
        public abstract Task<ReverseContinueResponse> Handle(ReverseContinueArguments request,
            CancellationToken cancellationToken);
    }

    public static class ReverseContinueExtensions
    {
        public static IDebugAdapterServerRegistry OnReverseContinue(this IDebugAdapterServerRegistry registry,
            Func<ReverseContinueArguments, CancellationToken, Task<ReverseContinueResponse>> handler)
        {
            return registry.AddHandler(RequestNames.ReverseContinue, RequestHandler.For(handler));
        }

        public static IDebugAdapterServerRegistry OnReverseContinue(this IDebugAdapterServerRegistry registry,
            Func<ReverseContinueArguments, Task<ReverseContinueResponse>> handler)
        {
            return registry.AddHandler(RequestNames.ReverseContinue, RequestHandler.For(handler));
        }

        public static Task<ReverseContinueResponse> RequestReverseContinue(this IDebugAdapterClient mediator, ReverseContinueArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
