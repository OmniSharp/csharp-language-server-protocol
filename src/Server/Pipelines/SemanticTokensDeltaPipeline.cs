using System.Diagnostics.CodeAnalysis;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Server.Pipelines
{
    internal class SemanticTokensDeltaPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse?>
        where TRequest : notnull
    {
        public async Task<TResponse?> Handle(TRequest request, RequestHandlerDelegate<TResponse?> next, CancellationToken cancellationToken)
        {
            if (request is SemanticTokensParams semanticTokensParams)
            {
                var response = await next().ConfigureAwait(false);
                if (GetResponse(semanticTokensParams, response, out var result) && string.IsNullOrEmpty(result.ResultId))
                {
                    result = result with { ResultId = Guid.NewGuid().ToString() };
                }

                return result is TResponse r ? r : response;
            }

            if (request is SemanticTokensDeltaParams semanticTokensDeltaParams)
            {
                var response = await next().ConfigureAwait(false);
                if (GetResponse(semanticTokensDeltaParams, response, out var result))
                {
                    if (result is { IsFull: true, Full: { ResultId: null or { Length: 0 } } })
                    {
                        result = result with { Full = result.Full with { ResultId = semanticTokensDeltaParams.PreviousResultId } };
                    }
                    else if (result is { IsDelta: true, Delta: { ResultId: null or { Length: 0 } } })
                    {
                        result = result with { Delta = result.Delta with { ResultId = semanticTokensDeltaParams.PreviousResultId } };
                    }
                }

                return result is TResponse r ? r : response;
            }

            return await next().ConfigureAwait(false);
        }

        // ReSharper disable once UnusedParameter.Local
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
