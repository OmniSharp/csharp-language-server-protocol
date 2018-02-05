using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server.Matchers
{
    public class ResolveCommandMatcher : IHandlerMatcher, IHandlerPostProcessorMatcher, IHandlerPostProcessor
    {
        private readonly ILogger _logger;

        public ResolveCommandMatcher(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Finds the first handler that matches the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="descriptors">The descriptors.</param>
        /// <returns></returns>
        public IEnumerable<ILspHandlerDescriptor> FindHandler(object parameters, IEnumerable<ILspHandlerDescriptor> descriptors)
        {
            if (parameters is ICanBeResolved canBeResolved)
            {
                var handlerType = canBeResolved.Data?.Value<string>("handlerType");
                if (string.IsNullOrWhiteSpace(handlerType))
                {
                    var descriptor = descriptors.FirstOrDefault();
                    _logger.LogTrace(
                        "Resolve {Method} was called, but data did not have handle type defined.  Using Handler {HandlerType}",
                        descriptor?.Method,
                        descriptor?.Handler.GetType().FullName
                    );

                    yield return descriptor;
                    yield break;
                }
                foreach (var descriptor in descriptors)
                {
                    _logger.LogTrace("Checking handler {Method}:{Handler}",
                        descriptor.Method,
                        descriptor.Handler.GetType().FullName);
                    if (descriptor.Handler.GetType().FullName == handlerType || descriptor.HandlerType.FullName == handlerType)
                    {
                        canBeResolved.Data = canBeResolved.Data["data"];
                        yield return descriptor;
                    }
                }
            }
        }

        public IEnumerable<IHandlerPostProcessor> FindPostProcessor(ILspHandlerDescriptor descriptor, object parameters, object response)
        {
            if (descriptor.Method == DocumentNames.CodeLens || descriptor.Method == DocumentNames.Completion)
            {
                _logger.LogTrace("Using handler {Method}:{Handler}",
                    descriptor.Method,
                    descriptor.Handler.GetType().FullName);
                yield return this;
            }
        }

        public object Process(ILspHandlerDescriptor descriptor, object parameters, object response)
        {
            if (response is IEnumerable<ICanBeResolved> canBeResolveds)
            {
                _logger.LogTrace("Updating Resolve items with wrapped data for {Method}:{Handler}",
                    descriptor.Method,
                    descriptor.Handler.GetType().FullName);
                foreach (var item in canBeResolveds)
                {
                    // Originally we were going to change Data to be a JObject instead of JToken
                    // This allows us to leave data alone by simply wrapping it
                    // Since we're always going to intercept these items, we can control this.
                    item.Data = new JObject(new {
                        data = item.Data,
                        handlerType = descriptor.HandlerType.FullName
                    });
                }
            }
            return response;
        }
    }
}
