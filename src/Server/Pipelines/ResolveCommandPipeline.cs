using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;

namespace OmniSharp.Extensions.LanguageServer.Server.Pipelines
{
    public class ResolveCommandPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<ResolveCommandPipeline<TRequest, TResponse>> _logger;
        private readonly ILspHandlerDescriptor _descriptor;

        public ResolveCommandPipeline(IRequestContext context, ILogger<ResolveCommandPipeline<TRequest, TResponse>> logger)
        {
            _logger = logger;
            _descriptor = ( context.Descriptor as ILspHandlerDescriptor )!;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next(cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            // Only pin the handler type, if we know the source handler (codelens) is also the resolver.
            if (response is IEnumerable<ICanBeResolved> canBeResolvedItems)
            {
                var id = _descriptor.Handler is ICanBeIdentifiedHandler resolved ? resolved.Id : Guid.Empty;
                _logger.LogTrace(
                    "Updating Resolve items with wrapped data for {Method}:{Handler}",
                    _descriptor.Method,
                    _descriptor.ImplementationType.FullName
                );
                foreach (var item in canBeResolvedItems)
                {
                    UpdatePrivateHandlerId(item, id);
                }
            }

            // Only pin the handler type, if we know the source handler (codelens) is also the resolver.
            if (response is ICanBeResolved canBeResolvedItem)
            {
                var id = _descriptor.Handler is ICanBeIdentifiedHandler resolved ? resolved.Id : Guid.Empty;
                _logger.LogTrace(
                    "Updating Resolve items with wrapped data for {Method}:{Handler}",
                    _descriptor.Method,
                    _descriptor.ImplementationType.FullName
                );
                UpdatePrivateHandlerId(canBeResolvedItem, id);
            }

            return response;

            void UpdatePrivateHandlerId(ICanBeResolved item, Guid id)
            {
                var data = item.Data is { ValueKind: JsonValueKind.Object }
                    ? item.Data.Value.Deserialize<Dictionary<string, JsonElement>>() ?? new Dictionary<string, JsonElement>()
                    : new Dictionary<string, JsonElement>();
                if (id == Guid.Empty)
                {
                    data.Remove(Constants.PrivateHandlerId);
                }
                else
                {
                    data[Constants.PrivateHandlerId] = JsonSerializer.SerializeToElement(id);
                }

                item.SetRawData(JsonSerializer.SerializeToElement(data));
            }
        }
    }
}
