using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server.Matchers
{
    public class ResolveCommandMatcher : IHandlerMatcher, IHandlerPreProcessorMatcher, IHandlerPostProcessorMatcher, IHandlerPreProcessor, IHandlerPostProcessor
    {
        private readonly ILogger _logger;
        internal static string PrivateHandlerTypeName = "$$___handlerType___$$";
        internal static string PrivateHandlerKey = "$$___handlerKey___$$";

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
                string handlerType = null;
                string handlerKey = null;
                if (canBeResolved.Data != null && canBeResolved.Data.Type == JTokenType.Object)
                {
                    handlerType = canBeResolved.Data?[PrivateHandlerTypeName]?.ToString();
                    handlerKey = canBeResolved.Data?[PrivateHandlerKey]?.ToString();
                }

                if (string.IsNullOrWhiteSpace(handlerType) &&
                    string.IsNullOrWhiteSpace(handlerKey))
                {
                    foreach (var descriptor in descriptors)
                    {
                        if (descriptor.CanBeResolvedHandlerType?.GetTypeInfo().IsAssignableFrom(descriptor.Handler.GetType()) == true)
                        {
                            var method = typeof(ResolveCommandMatcher).GetTypeInfo()
                                .GetMethod(nameof(CanResolve), BindingFlags.NonPublic | BindingFlags.Static)
                                .MakeGenericMethod(descriptor.Params);
                            if ((bool)method.Invoke(null, new[] { descriptor.Handler, parameters }))
                            {
                                yield return descriptor;
                                yield break;
                            }
                        }
                    }

                    var descriptor2 = descriptors.FirstOrDefault();
                    _logger.LogTrace(
                        "Resolve {Method} was called, but data did not have handle type defined.  Using Handler {HandlerType}",
                        descriptor2?.Method,
                        descriptor2?.Handler.GetType().FullName
                    );

                    yield return descriptor2;
                    yield break;
                }
                foreach (var descriptor in descriptors)
                {
                    _logger.LogTrace("Checking handler {Method}:{Handler}",
                        descriptor.Method,
                        descriptor.Handler.GetType().FullName);
                    if ((descriptor.Handler.GetType().FullName == handlerType || descriptor.HandlerType.FullName == handlerType) &&
                        ((descriptor is HandlerDescriptor handlerDescriptor) && handlerDescriptor.Key == handlerKey))
                    {
                        yield return descriptor;
                    }
                }
            }
        }

        private static bool CanResolve<T>(ICanBeResolvedHandler<T> handler, T value)
            where T : ICanBeResolved
        {
            return handler.CanResolve(value);
        }

        public IEnumerable<IHandlerPreProcessor> FindPreProcessor(ILspHandlerDescriptor descriptor, object parameters)
        {
            if (parameters is ICanBeResolved canBeResolved)
            {
                _logger.LogTrace("Using handler {Method}:{Handler}",
                    descriptor.Method,
                    descriptor.Handler.GetType().FullName);
                yield return this;
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

        public object Process(ILspHandlerDescriptor descriptor, object parameters)
        {
            if (parameters is ICanBeResolved canBeResolved)
            {
                string handlerType = null;
                if (canBeResolved.Data != null && canBeResolved.Data.Type == JTokenType.Object)
                    handlerType = canBeResolved.Data?[PrivateHandlerTypeName]?.ToString();

                if (!string.IsNullOrWhiteSpace(handlerType))
                {
                    canBeResolved.Data = canBeResolved.Data["data"];
                }
            }

            return parameters;
        }

        public object Process(ILspHandlerDescriptor descriptor, object parameters, object response)
        {
            var registrationOptions = descriptor.Registration.RegisterOptions as TextDocumentRegistrationOptions;

            // Only pin the handler type, if we know the source handler (codelens) is also the resolver.
            if (registrationOptions?.DocumentSelector != null &&
                response is IEnumerable<ICanBeResolved> canBeResolveds &&
                descriptor?.CanBeResolvedHandlerType?.GetTypeInfo().IsAssignableFrom(descriptor.Handler.GetType()) == true)
            {
                _logger.LogTrace("Updating Resolve items with wrapped data for {Method}:{Handler}",
                    descriptor.Method,
                    descriptor.Handler.GetType().FullName);
                foreach (var item in canBeResolveds)
                {
                    // Originally we were going to change Data to be a JObject instead of JToken
                    // This allows us to leave data alone by simply wrapping it
                    // Since we're always going to intercept these items, we can control this.
                    var data = new JObject();
                    data["data"] = item.Data;
                    data[PrivateHandlerTypeName] = descriptor.Handler.GetType().FullName;
                    data[PrivateHandlerKey] = registrationOptions.DocumentSelector.ToString();
                    item.Data = data;
                }
            }
            return response;
        }
    }
}
