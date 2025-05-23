using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
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
            var response = await next().ConfigureAwait(false);
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
                item.SetRawData(item.Data ?? new JObject());
                if (item.Data is JObject o)
                {
                    if (id == Guid.Empty)
                    {
                        if (o.ContainsKey(Constants.PrivateHandlerId))
                        {
                            o.Remove(Constants.PrivateHandlerId);
                        }

                        return;
                    }

                    o[Constants.PrivateHandlerId] = id;
                }
            }
        }
    }
}
