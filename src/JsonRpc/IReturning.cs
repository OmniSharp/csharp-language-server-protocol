using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IResponseRouterReturns
    {
        Task<TResponse> Returning<TResponse>(CancellationToken cancellationToken);
        Task ReturningVoid(CancellationToken cancellationToken);
    }
}
