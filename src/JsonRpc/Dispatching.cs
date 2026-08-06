using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IJsonRpcRequest<out TResponse>
    {
    }

    public interface IJsonRpcNotification
    {
    }

    public delegate Task<TResponse> JsonRpcRequestHandlerDelegate<TResponse>(CancellationToken cancellationToken);

    public delegate Task JsonRpcRequestHandlerDelegate(CancellationToken cancellationToken);

    public interface IJsonRpcPipelineBehavior<in TRequest, TResponse>
        where TRequest : IJsonRpcRequest<TResponse>
    {
        Task<TResponse> Handle(
            TRequest request,
            JsonRpcRequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken
        );
    }

    public interface IJsonRpcPipelineBehavior<in TRequest>
        where TRequest : IJsonRpcRequest
    {
        Task Handle(TRequest request, JsonRpcRequestHandlerDelegate next, CancellationToken cancellationToken);
    }

    public interface IJsonRpcNotificationPipelineBehavior<in TNotification>
        where TNotification : IJsonRpcNotification
    {
        Task Handle(TNotification notification, JsonRpcRequestHandlerDelegate next, CancellationToken cancellationToken);
    }
}