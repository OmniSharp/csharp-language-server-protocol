using System;
using System.Collections.Generic;
using System.Linq;
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
        internal static string PrivateHandlerTypeName = "$$___handlerType___$$";
        internal static string PrivateHandlerKey = "$$___handlerKey___$$";
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
            if (_descriptor is LspHandlerDescriptor handlerDescriptor && handlerDescriptor.CanBeResolved && response is IEnumerable<object> values)
            {
                PropertyInfo propertyInfo = null;
                _logger.LogTrace(
                    "Updating Resolve items with wrapped data for {Method}:{Handler}",
                    _descriptor.Method,
                    _descriptor.ImplementationType.FullName);
                foreach (var item in values)
                {
                    if (!(item is ICanBeResolved<CanBeResolvedData> value)) continue;
                    if (value.Data == null)
                    {
                        propertyInfo ??= item.GetType().GetProperty(nameof(value.Data));
                        propertyInfo.SetValue(item, Activator.CreateInstance(propertyInfo.PropertyType));
                    }

                    value.Data.handler = _descriptor.Handler is ICanBeIdentifiedHandler resolved ? resolved.Id : Guid.Empty;
                }
            }

            return response;
        }
    }
}
