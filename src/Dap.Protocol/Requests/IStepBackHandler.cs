using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.StepBack, Direction.ClientToServer)]
    public interface IStepBackHandler : IJsonRpcRequestHandler<StepBackArguments, StepBackResponse>
    {
    }

    public abstract class StepBackHandler : IStepBackHandler
    {
        public abstract Task<StepBackResponse> Handle(StepBackArguments request, CancellationToken cancellationToken);
    }

    public static class StepBackExtensions
    {
        public static IDebugAdapterServerRegistry OnStepBack(this IDebugAdapterServerRegistry registry,
            Func<StepBackArguments, CancellationToken, Task<StepBackResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StepBack, RequestHandler.For(handler));
        }

        public static IDebugAdapterServerRegistry OnStepBack(this IDebugAdapterServerRegistry registry,
            Func<StepBackArguments, Task<StepBackResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StepBack, RequestHandler.For(handler));
        }

        public static Task<StepBackResponse> RequestStepBack(this IDebugAdapterClient mediator, StepBackArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
