using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IResponseRouter
    {
        Task SendNotification(string method);
        Task SendNotification<T>(string method, T @params);
        Task SendNotification(IRequest request);
        IResponseRouterReturns SendRequest<T>(string method, T @params);
        IResponseRouterReturns SendRequest(string method);
        Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
    }
}
