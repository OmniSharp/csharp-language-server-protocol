using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(RequestNames.StepBack)]
    public interface IStepBackHandler : IJsonRpcRequestHandler<StepBackArguments, StepBackResponse> { }

    public abstract class StepBackHandler : IStepBackHandler
    {
        public abstract Task<StepBackResponse> Handle(StepBackArguments request, CancellationToken cancellationToken);
    }

    public static class StepBackHandlerExtensions
    {
        public static IDisposable OnStepBack(this IDebugAdapterRegistry registry, Func<StepBackArguments, CancellationToken, Task<StepBackResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : StepBackHandler
        {
            private readonly Func<StepBackArguments, CancellationToken, Task<StepBackResponse>> _handler;

            public DelegatingHandler(Func<StepBackArguments, CancellationToken, Task<StepBackResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<StepBackResponse> Handle(StepBackArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
