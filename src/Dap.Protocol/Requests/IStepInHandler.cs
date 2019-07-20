using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(RequestNames.StepIn)]
    public interface IStepInHandler : IJsonRpcRequestHandler<StepInArguments, StepInResponse> { }

    public abstract class StepInHandler : IStepInHandler
    {
        public abstract Task<StepInResponse> Handle(StepInArguments request, CancellationToken cancellationToken);
    }

    public static class StepInHandlerExtensions
    {
        public static IDisposable OnStepIn(this IDebugAdapterRegistry registry, Func<StepInArguments, CancellationToken, Task<StepInResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : StepInHandler
        {
            private readonly Func<StepInArguments, CancellationToken, Task<StepInResponse>> _handler;

            public DelegatingHandler(Func<StepInArguments, CancellationToken, Task<StepInResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<StepInResponse> Handle(StepInArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
