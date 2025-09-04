using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Server.Matchers
{
    public class ResolveCommandMatcher : IHandlerMatcher
    {
        private readonly ILogger<ResolveCommandMatcher> _logger;

        public ResolveCommandMatcher(ILogger<ResolveCommandMatcher> logger) => _logger = logger;

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
                if (canBeResolved.Data != null && canBeResolved.Data is JObject jObject && jObject.TryGetValue(Constants.PrivateHandlerId, out var value))
                {
                    if (!Guid.TryParse(value.ToString(), out var id)) yield break;

                    foreach (var descriptor in descriptors)
                    {
                        _logger.LogTrace("Checking handler {Method}:{Handler}", descriptor.Method, descriptor.ImplementationType.FullName);
                        // If they `ICanBeIdentifiedHandler` is implemented, use that.
                        if (descriptor.Handler is ICanBeIdentifiedHandler handler && handler.Id == id)
                        {
                            yield return descriptor;
                        }
                        // If we are a legacy resolve handler and we have no id set then continue on.
                        else if (descriptor.Handler is ICanBeResolvedHandler && id == Guid.Empty)
                        {
                            yield return descriptor;
                        }
                    }
                }
            }
        }
    }
}
