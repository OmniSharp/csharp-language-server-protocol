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
    class SemanticTokensDeltaPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is SemanticTokensParams semanticTokensParams)
            {
                var response = await next().ConfigureAwait(false);
                if (GetResponse(semanticTokensParams, response, out var result) && string.IsNullOrEmpty(result.ResultId))
                {
                    result.ResultId = Guid.NewGuid().ToString();
                }
                return response;
            }

            if (request is SemanticTokensDeltaParams semanticTokensDeltaParams)
            {
                var response = await next().ConfigureAwait(false);
                if (GetResponse(semanticTokensDeltaParams, response, out var result))
                {
                    if (result.Value.IsFull && string.IsNullOrEmpty(result.Value.Full!.ResultId))
                    {
                        result.Value.Full.ResultId = semanticTokensDeltaParams.PreviousResultId;
                    }

                    if (result.Value.IsDelta && string.IsNullOrEmpty(result.Value.Delta!.ResultId))
                    {
                        result.Value.Delta.ResultId = semanticTokensDeltaParams.PreviousResultId;
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
