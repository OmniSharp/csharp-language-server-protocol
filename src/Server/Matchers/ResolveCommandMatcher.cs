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
        internal static string PrivateHandlerId = "$$__handler_id__$$";

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
            if (parameters is ICanBeResolved canBeResolved)
            {
                if (canBeResolved.Data != null && canBeResolved.Data is JObject jObject && jObject.TryGetValue(PrivateHandlerId, out var value))
                {
                    var id = value.Value<Guid>();

                    foreach (var descriptor in descriptors)
                    {
                        _logger.LogTrace("Checking handler {Method}:{Handler}", descriptor.Method, descriptor.ImplementationType.FullName);
                        if (descriptor.Handler is ICanBeIdentifiedHandler handler && handler.Id != Guid.Empty && handler.Id == id)
                        {
                            yield return descriptor;
                        }
                    }
                }
            }
        }
    }
}
