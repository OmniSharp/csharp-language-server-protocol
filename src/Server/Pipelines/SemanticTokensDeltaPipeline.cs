using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;

namespace OmniSharp.Extensions.LanguageServer.Server.Pipelines
{
    [Obsolete(Constants.Proposal)]
    class SemanticTokensDeltaPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse?>
        where TRequest : notnull
        where TResponse : class?
    {
        public async Task<TResponse?> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse?> next)
        {
            if (request is SemanticTokensParams semanticTokensParams)
            {
                var response = await next().ConfigureAwait(false);
                if (GetResponse(semanticTokensParams, response, out var result) && string.IsNullOrEmpty(result.ResultId))
                {
                    return result with { ResultId = Guid.NewGuid().ToString() } as TResponse;
                }

                return response;
            }

            if (request is SemanticTokensDeltaParams semanticTokensDeltaParams)
            {
                var response = await next().ConfigureAwait(false);
                if (GetResponse(semanticTokensDeltaParams, response, out var result))
                {
                    if (result.IsFull && string.IsNullOrEmpty(result.Full!.ResultId))
                    {
                        return result with { Full = result.Full with { ResultId = semanticTokensDeltaParams.PreviousResultId } } as TResponse;
                    }

                    if (result.IsDelta && string.IsNullOrEmpty(result.Delta!.ResultId))
                    {
                        return result with { Delta = result.Delta with {ResultId = semanticTokensDeltaParams.PreviousResultId} } as TResponse;
                    }
                }

                return response;
            }

            return await next().ConfigureAwait(false);
        }

        private bool GetResponse<TR>(IRequest<TR> request, object? response, [NotNullWhen(true)] out TR result)
        {
            if (response is TR r)
            {
                result = r;
                return true;
            }

            result = default!;
            return false;
        }
    }
}
