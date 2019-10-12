using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Initialize)]
    public interface IInitializeHandler : IJsonRpcRequestHandler<InitializeRequestArguments, InitializeResponse> { }

    public abstract class InitializeHandler : IInitializeHandler
    {
        public abstract Task<InitializeResponse> Handle(InitializeRequestArguments request, CancellationToken cancellationToken);
    }

    public static class InitializeHandlerExtensions
    {
        public static IDisposable OnInitialize(this IDebugAdapterRegistry registry, Func<InitializeRequestArguments, CancellationToken, Task<InitializeResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : InitializeHandler
        {
            private readonly Func<InitializeRequestArguments, CancellationToken, Task<InitializeResponse>> _handler;

            public DelegatingHandler(Func<InitializeRequestArguments, CancellationToken, Task<InitializeResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<InitializeResponse> Handle(InitializeRequestArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
