using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using DryIoc;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace NSubstitute
{
    internal static class DryIocExtensions
    {
        /// <summary>
        /// Adds support for resolving generic ILoggers from the DI Container and allowing the loggers to be wrapped
        /// if desired
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        internal static Rules WithTestLoggerResolver(this Rules rules, Func<Request, Type, object> creator)
        {
            var dictionary = new ConcurrentDictionary<Type, Factory>();
            return rules.WithUnknownServiceResolvers(
                ( rules.UnknownServiceResolvers ?? Array.Empty<Rules.UnknownServiceResolver>() ).ToImmutableList().Add(
                    request => {
                        var serviceType = request.ServiceType;
                        if (!serviceType.IsInterface || !serviceType.IsGenericType ||
                            serviceType.GetGenericTypeDefinition() != typeof(ILogger<>))
                        {
                            return null;
                        }

                        if (!dictionary.TryGetValue(serviceType, out var instance))
                        {
                            var loggerType = typeof(Logger<>).MakeGenericType(
                                request.ServiceType.GetGenericArguments()[0]
                            );
                            instance = new DelegateFactory(_ => creator(request, loggerType), Reuse.Singleton);
                            dictionary.TryAdd(serviceType, instance);
                        }

                        return instance;
                    }
                ).ToArray()
            );
        }

        /// <summary>
        /// Adds support for auto stub/mock/faking dependencies that are not already in the container.
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        internal static Rules WithUndefinedTestDependenciesResolver(this Rules rules, Func<Request, object> creator)
        {
            var dictionary = new ConcurrentDictionary<Type, Factory>();
            return rules.WithUnknownServiceResolvers(
                ( rules.UnknownServiceResolvers ?? Array.Empty<Rules.UnknownServiceResolver>() ).ToImmutableList().Add(
                    request => {
                        var serviceType = request.ServiceType;
                        if (!serviceType.IsAbstract)
                            return null; // Mock interface or abstract class only.

                        if (request.Is(parameter: info => info.IsOptional))
                            return null; // Ignore optional parameters

                        if (!dictionary.TryGetValue(serviceType, out var instance))
                        {
                            instance = new DelegateFactory(_ => creator(request), Reuse.Singleton);
                            dictionary.TryAdd(serviceType, instance);
                        }

                        return instance;
                    }
                ).ToArray()
            );
        }
    }
}
