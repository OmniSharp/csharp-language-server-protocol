using System;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.ResolveAnything;

// ReSharper disable once CheckNamespace
namespace NSubstitute.Internals
{
    /// <summary>
    /// Auto mocking container using <see cref="Autofac"/> and <see cref="NSubstitute"/>.
    /// </summary>
    public class AutoSubstitute : IDisposable
    {

        /// <summary>
        /// Create an AutoSubstitute, but modify the <see cref="Autofac.ContainerBuilder"/> before building a container.
        /// </summary>
        /// <param name="builderModifier">Action to modify the <see cref="Autofac.ContainerBuilder"/></param>
        public AutoSubstitute(Action<ContainerBuilder> builderModifier)
        {
            var builder = new ContainerBuilder();

            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            builder.RegisterSource(new NSubstituteRegistrationHandler());

            builderModifier(builder);

            Container = builder.Build();
        }

        /// <summary>
        /// <see cref="IContainer"/> that handles the component resolution.
        /// </summary>
        public IContainer Container { get; }

        /// <summary>
        /// Cleans up the <see cref="Autofac.Core.Container"/>.
        /// </summary>
        public void Dispose()
        {
            Container.Dispose();
        }

        /// <summary>
        /// Resolve the specified type from the container.
        /// </summary>
        /// <typeparam name="T">The type to resolve</typeparam>
        /// <param name="parameters">Optional constructor parameters</param>
        /// <returns>The resolved object</returns>
        public T Resolve<T>(params Parameter[] parameters)
        {
            return Container.Resolve<T>(parameters);
        }

        /// <summary>
        /// Register the specified implementation type to the container as the specified service type and resolve it using the given parameters.
        /// </summary>
        /// <typeparam name="TService">The type to register the implementation as</typeparam>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        /// <param name="parameters">Optional constructor parameters</param>
        /// <returns>The resolved service instance</returns>
        public TService Provide<TService, TImplementation>(params Parameter[] parameters)
        {
            Container.ComponentRegistry.Register(RegistrationBuilder.ForType<TImplementation>()
                .As<TService>().InstancePerLifetimeScope().CreateRegistration()
            );

            return Container.Resolve<TService>(parameters);
        }

        /// <summary>
        /// Register the specified object to the container as the specified service type and resolve it.
        /// </summary>
        /// <typeparam name="TService">The type to register the object as</typeparam>
        /// <param name="instance">The object to register into the container</param>
        /// <returns>The instance resolved from container</returns>
        public TService Provide<TService>(TService instance)
            where TService : class
        {
            Container.ComponentRegistry.Register(RegistrationBuilder.ForDelegate((c, p) => instance)
                .InstancePerLifetimeScope().CreateRegistration()
            );

            return Container.Resolve<TService>();
        }

        /// <summary>
        /// Register the specified object to the container as the specified keyed service type and resolve it.
        /// </summary>
        /// <typeparam name="TService">The type to register the object as</typeparam>
        /// <param name="instance">The object to register into the container</param>
        /// <param name="serviceKey">The key to register the service with</param>
        /// <returns>The instance resolved from container</returns>
        public TService Provide<TService>(TService instance, object serviceKey)
            where TService : class
        {
            Container.ComponentRegistry.Register(RegistrationBuilder.ForDelegate((c, p) => instance).As(new KeyedService(serviceKey, typeof(TService)))
                .InstancePerLifetimeScope().CreateRegistration()
            );

            return Container.Resolve<TService>();
        }

        /// <summary>
        /// Registers to the container and returns a substitute for a given concrete class given the explicit constructor parameters.
        /// This is used for concrete classes where NSubstitutes won't be created by default by the container when using Resolve.
        /// For advanced uses consider using directly <see cref="Substitute.For{TService}"/> and then calling <see cref="Provide{TService}(TService)"/> so that type is used on dependencies for other Resolved types.
        /// </summary>
        /// <typeparam name="TService">The type to register and return a substitute for</typeparam>
        /// <param name="parameters">Optional constructor parameters</param>
        /// <returns>The instance resolved from the container</returns>
        public TService For<TService>(params object[] parameters) where TService : class
        {
            var substitute = Substitute.For<TService>(parameters);
            return Provide(substitute);
        }

        /// <summary>
        /// Registers to the container and returns a substitute for a given concrete class using autofac to resolve the constructor parameters.
        /// This is used for concrete classes where NSubstitutes won't be created by default by the container when using Resolve.
        /// For advanced uses consider using directly <see cref="Substitute.For{TService}"/> and then calling <see cref="Provide{TService}(TService)"/> so that type is used on dependencies for other Resolved types.
        /// </summary>
        /// <typeparam name="TService">The type to register and return a substitute for</typeparam>
        /// <param name="parameters">Any constructor parameters that Autofac can't resolve automatically</param>
        /// <returns>The instance resolved from the container</returns>
        public TService For<TService>(params Parameter[] parameters) where TService : class
        {
            var substitute = Resolve<TService>(parameters);
            return Provide(substitute);
        }
    }
}
