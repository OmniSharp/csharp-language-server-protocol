using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace NSubstitute.Internals
{
/// <summary>
    /// Automatically creates substitute for requested services that haven't been registered
    /// </summary>
    public class AutoSubstitute : IDisposable
    {
        /// <summary>
        /// Create a container that automatically substitutes unknown types.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="configureAction"></param>
        public AutoSubstitute(
            IContainer? container = null,
            Func<IContainer, IContainer>? configureAction = null)
        {
            Container = container ?? new Container();

            Container = Container

                .With(rules => rules
                        .WithTestLoggerResolver((request, loggerType) => ActivatorUtilities.CreateInstance(request.Container, loggerType))
                        .WithUndefinedTestDependenciesResolver(request => Substitute.For(new[] { request.ServiceType }, null))
                        .WithConcreteTypeDynamicRegistrations((type, o) => true, Reuse.Transient)
                );

            if (configureAction != null)
            {
                Container = configureAction.Invoke(Container);
            }
        }

        /// <summary>
        /// Gets the <see cref="IContainer"/> that handles the component resolution.
        /// </summary>
        public IContainer Container { get; }

        /// <summary>
        /// Resolve the specified type in the container (register it if needed).
        /// </summary>
        /// <typeparam name="T">The type of the service.</typeparam>
        /// <returns>The service.</returns>
        public T Resolve<T>() => Container.Resolve<T>();

        /// <summary>
        /// Resolve the specified type in the container (register specified instance if needed).
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance to register if needed.</param>
        /// <returns>The instance resolved from container.</returns>
        [SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "The component registry is responsible for registration disposal."
        )]
        public TService Provide<TService>(TService instance)
            where TService : class
        {
            Container.RegisterInstance(instance);
            return instance;
        }

        /// <summary>
        /// Resolve the specified type in the container (register specified instance if needed).
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <returns>The instance resolved from container.</returns>
        [SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "The component registry is responsible for registration disposal."
        )]
        public TService Provide<TService, TImplementation>() where TImplementation : TService
        {
            Container.Register<TService, TImplementation>();
            return Container.Resolve<TService>();
        }

        void IDisposable.Dispose() => Container.Dispose();
    }

    public static class DryIocExtensions
    {
        /// <summary>
        /// Adds support for resolving generic ILoggers from the DI Container and allowing the loggers to be wrapped
        /// if desired
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public static Rules WithTestLoggerResolver(this Rules rules, Func<Request, Type, object> creator)
        {
            var dictionary = new ConcurrentDictionary<Type, Factory>();
            return rules.WithUnknownServiceResolvers(
                (rules.UnknownServiceResolvers ?? Array.Empty<Rules.UnknownServiceResolver>()).ToImmutableList().Add(
                    request =>
                    {
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
        public static Rules WithUndefinedTestDependenciesResolver(this Rules rules, Func<Request, object> creator)
        {
            var dictionary = new ConcurrentDictionary<Type, Factory>();
            return rules.WithUnknownServiceResolvers(
                (rules.UnknownServiceResolvers ?? Array.Empty<Rules.UnknownServiceResolver>()).ToImmutableList().Add(
                    request =>
                    {
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
