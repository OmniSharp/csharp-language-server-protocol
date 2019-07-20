using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(RequestNames.StepInTargets)]
    public interface IStepInTargetsHandler : IJsonRpcRequestHandler<StepInTargetsArguments, StepInTargetsResponse> { }

    public abstract class StepInTargetsHandler : IStepInTargetsHandler
    {
        public abstract Task<StepInTargetsResponse> Handle(StepInTargetsArguments request, CancellationToken cancellationToken);
    }

    public static class StepInTargetsHandlerExtensions
    {
        public static IDisposable OnStepInTargets(this IDebugAdapterRegistry registry, Func<StepInTargetsArguments, CancellationToken, Task<StepInTargetsResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : StepInTargetsHandler
        {
            private readonly Func<StepInTargetsArguments, CancellationToken, Task<StepInTargetsResponse>> _handler;

            public DelegatingHandler(Func<StepInTargetsArguments, CancellationToken, Task<StepInTargetsResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<StepInTargetsResponse> Handle(StepInTargetsArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
