using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc
{
    /// <summary>
    ///
    /// Server --> -->
    ///               |
    /// Client <-- <--
    ///
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IResponseHandler<TResponse> : IJsonRpcHandler
    {
        Task Handle(TResponse request);
    }
}