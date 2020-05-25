using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.StepInTargets, Direction.ClientToServer)]
    public interface IStepInTargetsHandler : IJsonRpcRequestHandler<StepInTargetsArguments, StepInTargetsResponse>
    {
    }

    public abstract class StepInTargetsHandler : IStepInTargetsHandler
    {
        public abstract Task<StepInTargetsResponse> Handle(StepInTargetsArguments request,
            CancellationToken cancellationToken);
    }

    public static class StepInTargetsExtensions
    {
        public static IDebugAdapterServerRegistry OnStepInTargets(this IDebugAdapterServerRegistry registry,
            Func<StepInTargetsArguments, CancellationToken, Task<StepInTargetsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StepInTargets, RequestHandler.For(handler));
        }

        public static IDebugAdapterServerRegistry OnStepInTargets(this IDebugAdapterServerRegistry registry,
            Func<StepInTargetsArguments, Task<StepInTargetsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StepInTargets, RequestHandler.For(handler));
        }

        public static Task<StepInTargetsResponse> RequestStepInTargets(this IDebugAdapterClient mediator, StepInTargetsArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
