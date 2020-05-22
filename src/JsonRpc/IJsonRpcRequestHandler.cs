using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    /// <summary>
    ///
    /// Client --> -->
    ///               |
    /// Server <-- <--
    ///
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IJsonRpcRequestHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse>, IJsonRpcHandler
        where TRequest : IRequest<TResponse>
    { }

    /// <summary>
    ///
    /// Client --> -->
    ///               |
    /// Server <-- <--
    ///
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    public interface IJsonRpcRequestHandler<in TRequest> : IRequestHandler<TRequest>, IJsonRpcHandler
        where TRequest : IRequest
    { }
}
