using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(RequestNames.Terminate)]
    public interface ITerminateHandler : IJsonRpcRequestHandler<TerminateArguments, TerminateResponse> { }

    public abstract class TerminateHandler : ITerminateHandler
    {
        public abstract Task<TerminateResponse> Handle(TerminateArguments request, CancellationToken cancellationToken);
    }

    public static class TerminateHandlerExtensions
    {
        public static IDisposable OnTerminate(this IDebugAdapterRegistry registry, Func<TerminateArguments, CancellationToken, Task<TerminateResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : TerminateHandler
        {
            private readonly Func<TerminateArguments, CancellationToken, Task<TerminateResponse>> _handler;

            public DelegatingHandler(Func<TerminateArguments, CancellationToken, Task<TerminateResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<TerminateResponse> Handle(TerminateArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
