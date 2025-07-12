using MediatR;

namespace OmniSharp.Extensions.JsonRpc.Testing
{
    public class SettlePipeline<T, TR> : IPipelineBehavior<T, TR>
        where T : IRequest<TR>
    {
        private readonly IRequestSettler _settler;

        public SettlePipeline(IRequestSettler settler) => _settler = settler;

        async Task<TR> IPipelineBehavior<T, TR>.Handle(T request, RequestHandlerDelegate<TR> next, CancellationToken cancellationToken)
        {
            _settler.OnStartRequest();
            try
            {
                return await next().ConfigureAwait(false);
            }
            finally
            {
                _settler.OnEndRequest();
            }
        }
    }
}
