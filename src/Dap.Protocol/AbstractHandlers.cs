using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public static class AbstractHandlers
    {
        public abstract class Request<TParams, TResult> :
            IJsonRpcRequestHandler<TParams, TResult>
            where TParams : IRequest<TResult>
        {
            public abstract Task<TResult> Handle(TParams request, CancellationToken cancellationToken);
        }

        public abstract class Notification<TParams> : IJsonRpcRequestHandler<TParams>
            where TParams : IRequest
        {
            public abstract Task<Unit> Handle(TParams request, CancellationToken cancellationToken);
        }
    }
}
