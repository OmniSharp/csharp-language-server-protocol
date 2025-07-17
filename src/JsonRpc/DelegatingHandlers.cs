using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class DelegatingHandlers
    {
        public class Request<TParams, TResult> :
            IJsonRpcRequestHandler<TParams, TResult>
            where TParams : IRequest<TResult>
        {
            private readonly Func<TParams, CancellationToken, Task<TResult>> _handler;

            public Request(Func<TParams, CancellationToken, Task<TResult>> handler) => _handler = handler;

            Task<TResult> IRequestHandler<TParams, TResult>.
                Handle(TParams request, CancellationToken cancellationToken) =>
                _handler(request, cancellationToken);
        }

        public class Request<TParams> :
            IJsonRpcRequestHandler<TParams>
            where TParams : IRequest<Unit>
        {
            private readonly Func<TParams, CancellationToken, Task> _handler;

            public Request(Func<TParams, CancellationToken, Task> handler) => _handler = handler;

            async Task<Unit> IRequestHandler<TParams, Unit>.
                Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }
        }

        public class Notification<TParams> : IJsonRpcRequestHandler<TParams>
            where TParams : IRequest<Unit>
        {
            private readonly Func<TParams, CancellationToken, Task> _handler;

            public Notification(Func<TParams, CancellationToken, Task> handler) => _handler = handler;

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }
        }
    }
}
