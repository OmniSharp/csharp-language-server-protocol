using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace OmniSharp.Extensions.JsonRpc.Testing
{
    public class SettlePipeline<T, R> : IPipelineBehavior<T, R>
        where T : IRequest<R>
    {
        private readonly IRequestSettler _settler;

        public SettlePipeline(IRequestSettler settler)
        {
            _settler = settler;
        }

        async Task<R> IPipelineBehavior<T, R>.Handle(T request, CancellationToken cancellationToken, RequestHandlerDelegate<R> next)
        {
            _settler.OnStartRequest();
            try
            {
                return await next();
            }
            finally
            {
                _settler.OnEndRequest();
            }
        }
    }
}
