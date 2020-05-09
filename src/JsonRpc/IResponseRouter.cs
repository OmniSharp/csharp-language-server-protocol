using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IResponseRouter
    {
        void SendNotification(string method);
        void SendNotification<T>(string method, T @params);
        void SendNotification(IRequest request);
        IResponseRouterReturns SendRequest<T>(string method, T @params);
        IResponseRouterReturns SendRequest(string method);
        Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
        TaskCompletionSource<Memory<byte>> GetRequest(long id);
    }
}
