using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.Embedded.MediatR
{
    /// <summary>
    /// Wrapper class for a handler that synchronously handles a request and returns a response
    /// </summary>
    /// <typeparam name="TRequest">The type of request being handled</typeparam>
    /// <typeparam name="TResponse">The type of response from the handler</typeparam>
    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> IRequestHandler<TRequest, TResponse>.Handle(TRequest request, CancellationToken cancellationToken)
            => Task.FromResult(Handle(request));

        /// <summary>
        /// Override in a derived class for the handler logic
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        protected abstract TResponse Handle(TRequest request);
    }
}