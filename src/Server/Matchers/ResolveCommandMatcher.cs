using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Server.Matchers
{
    public class ResolveCommandMatcher : IHandlerMatcher
    {
        private readonly ILogger<ResolveCommandMatcher> _logger;

        public ResolveCommandMatcher(ILogger<ResolveCommandMatcher> logger)
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
            if (parameters is ICanBeResolved<CanBeResolvedData> canBeResolved)
            {
                var handlerName = canBeResolved.Data?.handler;
                if (handlerName == null || handlerName == Guid.Empty)
                {
                    var descriptor2 = descriptors.FirstOrDefault();
                    _logger.LogTrace(
                        "Resolve {Method} was called, but data did not have handle type defined.  Using Handler {HandlerType}",
                        descriptor2?.Method,
                        descriptor2?.ImplementationType.FullName
                    );

                    yield return descriptor2;
                    yield break;
                }
                foreach (var descriptor in descriptors)
                {
                    _logger.LogTrace("Checking handler {Method}:{Handler}",
                        descriptor.Method,
                        descriptor.ImplementationType.FullName);
                    if (!(descriptor.Handler is ICanBeIdentifiedHandler handler)) continue;

                    if (handler.Id == handlerName)
                    {
                        yield return descriptor;
                    }
                }
            }
        }
    }
}
