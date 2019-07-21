using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Completions)]
    public interface ICompletionsHandler : IJsonRpcRequestHandler<CompletionsArguments, CompletionsResponse> { }

    public abstract class CompletionsHandler : ICompletionsHandler
    {
        public abstract Task<CompletionsResponse> Handle(CompletionsArguments request, CancellationToken cancellationToken);
    }

    public static class CompletionsHandlerExtensions
    {
        public static IDisposable OnCompletions(this IDebugAdapterRegistry registry, Func<CompletionsArguments, CancellationToken, Task<CompletionsResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : CompletionsHandler
        {
            private readonly Func<CompletionsArguments, CancellationToken, Task<CompletionsResponse>> _handler;

            public DelegatingHandler(Func<CompletionsArguments, CancellationToken, Task<CompletionsResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<CompletionsResponse> Handle(CompletionsArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }

}
