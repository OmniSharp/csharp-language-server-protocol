using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.JsonRpc
{
    [Parallel, Method(JsonRpcNames.CancelRequest)]
    public interface ICancelRequestHandler : IJsonRpcNotificationHandler<CancelParams> { }

    public abstract class CancelRequestHandler : ICancelRequestHandler
    {
        public abstract Task<Unit> Handle(CancelParams request, CancellationToken cancellationToken);
    }

    public static class CancelRequestHandlerExtensions
    {
        public static IDisposable OnCancelRequest(this IJsonRpcHandlerRegistry registry, Func<CancelParams, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : CancelRequestHandler
        {
            private readonly Func<CancelParams, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<CancelParams, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(CancelParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
