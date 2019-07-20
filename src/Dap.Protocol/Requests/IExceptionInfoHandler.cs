using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(RequestNames.ExceptionInfo)]
    public interface IExceptionInfoHandler : IJsonRpcRequestHandler<ExceptionInfoArguments, ExceptionInfoResponse> { }

    public abstract class ExceptionInfoHandler : IExceptionInfoHandler
    {
        public abstract Task<ExceptionInfoResponse> Handle(ExceptionInfoArguments request, CancellationToken cancellationToken);
    }

    public static class ExceptionInfoHandlerExtensions
    {
        public static IDisposable OnExceptionInfo(this IDebugAdapterRegistry registry, Func<ExceptionInfoArguments, CancellationToken, Task<ExceptionInfoResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ExceptionInfoHandler
        {
            private readonly Func<ExceptionInfoArguments, CancellationToken, Task<ExceptionInfoResponse>> _handler;

            public DelegatingHandler(Func<ExceptionInfoArguments, CancellationToken, Task<ExceptionInfoResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<ExceptionInfoResponse> Handle(ExceptionInfoArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }

}
