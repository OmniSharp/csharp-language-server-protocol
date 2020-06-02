using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.StepOut, Direction.ClientToServer)]
    public interface IStepOutHandler : IJsonRpcRequestHandler<StepOutArguments, StepOutResponse>
    {
    }

    public abstract class StepOutHandler : IStepOutHandler
    {
        public abstract Task<StepOutResponse> Handle(StepOutArguments request, CancellationToken cancellationToken);
    }

    public static class StepOutExtensions
    {
        public static IDebugAdapterServerRegistry OnStepOut(this IDebugAdapterServerRegistry registry,
            Func<StepOutArguments, CancellationToken, Task<StepOutResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StepOut, RequestHandler.For(handler));
        }

        public static IDebugAdapterServerRegistry OnStepOut(this IDebugAdapterServerRegistry registry,
            Func<StepOutArguments, Task<StepOutResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StepOut, RequestHandler.For(handler));
        }

        public static Task<StepOutResponse> RequestStepOut(this IDebugAdapterClient mediator, StepOutArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
