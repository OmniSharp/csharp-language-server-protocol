using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;

namespace OmniSharp.Extensions.LanguageServer.Server.Pipelines
{
    public class ResolveCommandPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<ResolveCommandPipeline<TRequest, TResponse>> _logger;
        internal static string PrivateHandlerId = "$$__handler_id__$$";
        private readonly ILspHandlerDescriptor _descriptor;

        public ResolveCommandPipeline(IRequestContext context, ILogger<ResolveCommandPipeline<TRequest, TResponse>> logger)
        {
            _logger = logger;
            _descriptor = context.Descriptor as ILspHandlerDescriptor;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();
            cancellationToken.ThrowIfCancellationRequested();

            // Only pin the handler type, if we know the source handler (codelens) is also the resolver.
            if (_descriptor is LspHandlerDescriptor handlerDescriptor &&
                response is IEnumerable<ICanBeResolved> canBeResolveds &&
                _descriptor?.CanBeResolvedHandlerType?.GetTypeInfo().IsAssignableFrom(_descriptor.ImplementationType) == true)
            {
                _logger.LogTrace(
                    "Updating Resolve items with wrapped data for {Method}:{Handler}",
                    _descriptor.Method,
                    _descriptor.ImplementationType.FullName);
                foreach (var item in canBeResolveds)
                {
                    item.Data ??= new JObject();
                    item.Data[PrivateHandlerId] = _descriptor.Handler is ICanBeIdentifiedHandler resolved ? resolved.Id : Guid.Empty;
                }
            }

            return response;
        }
    }
}
