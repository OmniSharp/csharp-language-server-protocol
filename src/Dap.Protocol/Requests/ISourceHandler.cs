using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(RequestNames.Source)]
    public interface ISourceHandler : IJsonRpcRequestHandler<SourceArguments, SourceResponse> { }

    public abstract class SourceHandler : ISourceHandler
    {
        public abstract Task<SourceResponse> Handle(SourceArguments request, CancellationToken cancellationToken);
    }

    public static class SourceHandlerExtensions
    {
        public static IDisposable OnSource(this IDebugAdapterRegistry registry, Func<SourceArguments, CancellationToken, Task<SourceResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : SourceHandler
        {
            private readonly Func<SourceArguments, CancellationToken, Task<SourceResponse>> _handler;

            public DelegatingHandler(Func<SourceArguments, CancellationToken, Task<SourceResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<SourceResponse> Handle(SourceArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
