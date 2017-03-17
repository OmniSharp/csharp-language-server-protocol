using System.Threading.Tasks;

namespace JsonRpc
{
    /// <summary>
    ///
    /// Server --> -->
    ///               |
    /// Client <-- <--
    ///
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IResponseHandler<TResponse>
    {
        Task Handle(TResponse request);
    }
}