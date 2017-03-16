using System.Threading.Tasks;

namespace JsonRPC
{
    public interface IRequestHandler<TRequest, TResponse>
    {
        Task<TResponse> Handle(TRequest request);
    }

    public interface IRequestHandler<TRequest>
    {
        Task Handle(TRequest request);
    }
}