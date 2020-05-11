using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server.Matchers
{
    public class ResolveCommandMatcher : IHandlerMatcher
    {
        private readonly ILogger<ResolveCommandMatcher> _logger;
        internal static string PrivateHandlerTypeName = "$$___handlerType___$$";
        internal static string PrivateHandlerKey = "$$___handlerKey___$$";

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
                string handlerType = null;
                string handlerKey = null;
                if (canBeResolved.Data.ValueKind == JsonValueKind.Object)
                {
                    handlerType = canBeResolved.Data.TryGetProperty(PrivateHandlerTypeName, out var ht)
                        ? ht.GetString()
                        : null;
                    handlerKey = canBeResolved.Data.TryGetProperty(PrivateHandlerKey, out var hk)
                        ? hk.GetString()
                        : null;
                }

                if (string.IsNullOrWhiteSpace(handlerType) &&
                    string.IsNullOrWhiteSpace(handlerKey))
                {
                    foreach (var descriptor in descriptors)
                    {
                        if (descriptor.Params == parameters.GetType())
                        // if (descriptor.CanBeResolvedHandlerType?.GetTypeInfo().IsAssignableFrom(descriptor.ImplementationType) == true)
                        {
                            var method = CanResolveMethod
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
                    if ((descriptor.ImplementationType.FullName == handlerType || descriptor.HandlerType.FullName == handlerType) &&
                        ((descriptor is HandlerDescriptor handlerDescriptor) && handlerDescriptor.Key == handlerKey))
                    {
                        yield return descriptor;
                    }
                }
            }
        }

        private static readonly MethodInfo CanResolveMethod =
            typeof(ResolveCommandMatcher).GetTypeInfo()
                .GetMethod(nameof(CanResolve), BindingFlags.NonPublic | BindingFlags.Static);

        private static bool CanResolve<T>(ICanBeResolvedHandler<T> handler, T value)
            where T : ICanBeResolved, IRequest<T>
        {
            return handler.CanResolve(value);
        }
    }
}
