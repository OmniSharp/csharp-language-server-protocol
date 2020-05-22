using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.StepIn, Direction.ClientToServer)]
    public interface IStepInHandler : IJsonRpcRequestHandler<StepInArguments, StepInResponse>
    {
    }

    public abstract class StepInHandler : IStepInHandler
    {
        public abstract Task<StepInResponse> Handle(StepInArguments request, CancellationToken cancellationToken);
    }

    public static class StepInExtensions
    {
        public static IDisposable OnStepIn(this IDebugAdapterServerRegistry registry,
            Func<StepInArguments, CancellationToken, Task<StepInResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StepIn, RequestHandler.For(handler));
        }

        public static IDisposable OnStepIn(this IDebugAdapterServerRegistry registry,
            Func<StepInArguments, Task<StepInResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StepIn, RequestHandler.For(handler));
        }

        public static Task<StepInResponse> RequestStepIn(this IDebugAdapterClient mediator, StepInArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
