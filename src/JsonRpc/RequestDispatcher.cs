using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace OmniSharp.Extensions.JsonRpc
{
    internal interface IRequestDispatcher
    {
        Task Send<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest<Unit>;

        Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest<TResponse>;
    }

    internal class RequestDispatcher : IRequestDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IRequestContext _requestContext;

        public RequestDispatcher(IServiceProvider serviceProvider, IRequestContext requestContext)
        {
            _serviceProvider = serviceProvider;
            _requestContext = requestContext;
        }

        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest<Unit> =>
            Send<TRequest, Unit>(request, cancellationToken);

        public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest<TResponse>
        {
            RequestHandlerDelegate<TResponse> handler = token =>
                ((IRequestHandler<TRequest, TResponse>)_requestContext.Descriptor.Handler).Handle(request, token);

            foreach (var behavior in _serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>().Reverse())
            {
                var next = handler;
                handler = token => behavior.Handle(request, next, token);
            }

            return handler(cancellationToken);
        }
    }
}