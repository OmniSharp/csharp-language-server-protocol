using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

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
            if (request is ICanBeResolved canBeResolved)
            {
                string handlerType = null;
                if (canBeResolved.Data != null && canBeResolved.Data.Type == JTokenType.Object)
                    handlerType = canBeResolved.Data?[PrivateHandlerTypeName]?.ToString();

                if (!string.IsNullOrWhiteSpace(handlerType))
                {
                    canBeResolved.Data = canBeResolved.Data["data"];
                }
            }

            var response = await next();

            // Only pin the handler type, if we know the source handler (codelens) is also the resolver.
            if (_descriptor is HandlerDescriptor handlerDescriptor &&
                response is IEnumerable<ICanBeResolved> canBeResolveds &&
                _descriptor?.CanBeResolvedHandlerType?.GetTypeInfo().IsAssignableFrom(_descriptor.Handler.GetType()) == true)
            {
                _logger.LogTrace(
                    "Updating Resolve items with wrapped data for {Method}:{Handler}",
                    _descriptor.Method,
                    _descriptor.Handler.GetType().FullName);
                foreach (var item in canBeResolveds)
                {
                    // Originally we were going to change Data to be a JObject instead of JToken
                    // This allows us to leave data alone by simply wrapping it
                    // Since we're always going to intercept these items, we can control this.
                    var data = new JObject();
                    data["data"] = item.Data;
                    data[PrivateHandlerTypeName] = _descriptor.Handler.GetType().FullName;
                    data[PrivateHandlerKey] = handlerDescriptor.Key;
                    item.Data = data;
                }
            }

            return response;
        }
    }
}
