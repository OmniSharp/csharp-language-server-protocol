using System.Threading;
using System.Threading.Tasks;

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
    public interface IRequestHandler<TRequest, TResponse> : IJsonRpcHandler
    {
        Task<TResponse> Handle(TRequest request, CancellationToken token);
    }

    /// <summary>
    ///
    /// Client --> -->
    ///               |
    /// Server <-- <--
    ///
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    public interface IRequestHandler<TRequest> : IJsonRpcHandler
    {
        Task Handle(TRequest request, CancellationToken token);
    }
}