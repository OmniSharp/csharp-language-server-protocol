using System.Diagnostics.CodeAnalysis;
using DryIoc;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace NSubstitute.Internals
{
    /// <summary>
    /// Automatically creates substitute for requested services that haven't been registered
    /// </summary>
    internal class AutoSubstitute : IDisposable
    {
        /// <summary>
        /// Create a container that automatically substitutes unknown types.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="configureAction"></param>
        internal AutoSubstitute(
            IContainer? container = null,
            Func<IContainer, IContainer>? configureAction = null
        )
        {
            Container = container ?? new Container();

            Container = Container
               .With(
                    rules => rules
                            .WithTestLoggerResolver((request, loggerType) => request.Container.Resolve(loggerType))
                            .WithUndefinedTestDependenciesResolver(request => Substitute.For(new[] { request.ServiceType }, null))
                            .WithConcreteTypeDynamicRegistrations((type, o) => true, Reuse.Transient)
                );

            if (configureAction != null)
            {
                Container = configureAction.Invoke(Container);
            }
        }

        /// <summary>
        /// Gets the <see cref="IContainer" /> that handles the component resolution.
        /// </summary>
        internal IContainer Container { get; }

        /// <summary>
        /// Resolve the specified type in the container (register it if needed).
        /// </summary>
        /// <typeparam name="T">The type of the service.</typeparam>
        /// <returns>The service.</returns>
        public T Resolve<T>() => Container.GetRequiredService<T>();

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
}
