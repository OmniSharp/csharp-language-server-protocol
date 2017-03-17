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
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IResponseHandler<TRequest, TResponse>
    {
        Task Handle(TResponse request);
    }

    /// <summary>
    /// 
    /// Server --> -->
    ///               |
    /// Client <-- <--
    /// 
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IResponseHandler<TRequest>
    {
        Task Handle();
    }
}