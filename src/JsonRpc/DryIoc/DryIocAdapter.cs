using System;
using System.Collections.Generic;
using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

// ReSharper disable once CheckNamespace
namespace DryIoc
{

    /// <summary>Adapts DryIoc container to be used as MS.DI service provider, plus provides the helpers
    /// to simplify work with adapted container.</summary>
    internal static class DryIocAdapter
    {
        /// <summary>Creates the container and the `IServiceProvider` because its implemented by `IContainer` -
        /// you get simply the best of both worlds.</summary>
        public static IContainer Create(
            IEnumerable<ServiceDescriptor> services,
            Func<IRegistrator, ServiceDescriptor, bool> registerService = null)
        {
            var container = new Container(Rules.MicrosoftDependencyInjectionRules);

            container.Use<IServiceScopeFactory>(r => new DryIocServiceScopeFactory(r));
            container.Populate(services, registerService);

            return container;
        }

        /// <summary>Adapts passed <paramref name="container"/> to Microsoft.DependencyInjection conventions,
        /// registers DryIoc implementations of <see cref="IServiceProvider"/> and <see cref="IServiceScopeFactory"/>,
        /// and returns NEW container.
        /// </summary>
        /// <param name="container">Source container to adapt.</param>
        /// <param name="descriptors">(optional) Specify service descriptors or use <see cref="Populate"/> later.</param>
        /// <param name="registerDescriptor">(optional) Custom registration action, should return true to skip normal registration.</param>
        /// <example>
        /// <code><![CDATA[
        ///
        ///     var container = new Container();
        ///
        ///     // you may register the services here:
        ///     container.Register<IMyService, MyService>(Reuse.Scoped)
        ///
        ///     var adaptedContainer = container.WithDependencyInjectionAdapter(services);
        ///     IServiceProvider serviceProvider = adaptedContainer; // the container implements IServiceProvider now
        ///
        ///]]></code>
        /// </example>
        /// <remarks>You still need to Dispose adapted container at the end / application shutdown.</remarks>
        public static IContainer WithDependencyInjectionAdapter(this IContainer container,
            IEnumerable<ServiceDescriptor> descriptors = null,
            Func<IRegistrator, ServiceDescriptor, bool> registerDescriptor = null)
        {
            if (container.Rules != Rules.MicrosoftDependencyInjectionRules)
                container = container.With(rules => rules.WithMicrosoftDependencyInjectionRules());

            container.Use<IServiceScopeFactory>(r => new DryIocServiceScopeFactory(r));

            // Registers service collection
            if (descriptors != null)
                container.Populate(descriptors, registerDescriptor);

            return container;
        }

        /// <summary>Adds services registered in <paramref name="compositionRootType"/> to container</summary>
        public static IContainer WithCompositionRoot(this IContainer container, Type compositionRootType)
        {
            container.Register(compositionRootType);
            container.Resolve(compositionRootType);
            return container;
        }

        /// <summary>Adds services registered in <typeparamref name="TCompositionRoot"/> to container</summary>
        public static IContainer WithCompositionRoot<TCompositionRoot>(this IContainer container) =>
            container.WithCompositionRoot(typeof(TCompositionRoot));

        /// <summary>It does not really build anything, it just gets the `IServiceProvider` from the container.</summary>
        public static IServiceProvider BuildServiceProvider(this IContainer container) =>
            container.GetServiceProvider();

        /// <summary>Just gets the `IServiceProvider` from the container.</summary>
        public static IServiceProvider GetServiceProvider(this IResolver container) =>
            container;

        /// <summary>Facade to consolidate DryIoc registrations in <typeparamref name="TCompositionRoot"/></summary>
        /// <typeparam name="TCompositionRoot">The class will be created by container on Startup
        /// to enable registrations with injected <see cref="IRegistrator"/> or full <see cref="IContainer"/>.</typeparam>
        /// <param name="container">Adapted container</param> <returns>Service provider</returns>
        /// <example>
        /// <code><![CDATA[
        /// internal class ExampleCompositionRoot
        /// {
        ///    // if you need the whole container then change parameter type from IRegistrator to IContainer
        ///    public ExampleCompositionRoot(IRegistrator r)
        ///    {
        ///        r.Register<ISingletonService, SingletonService>(Reuse.Singleton);
        ///        r.Register<ITransientService, TransientService>(Reuse.Transient);
        ///        r.Register<IScopedService, ScopedService>(Reuse.InCurrentScope);
        ///    }
        /// }
        /// ]]></code>
        /// </example>
        public static IServiceProvider ConfigureServiceProvider<TCompositionRoot>(this IContainer container) =>
            container.WithCompositionRoot<TCompositionRoot>().GetServiceProvider();

        /// <summary>Registers service descriptors into container. May be called multiple times with different service collections.</summary>
        /// <param name="container">The container.</param>
        /// <param name="descriptors">The service descriptors.</param>
        /// <param name="registerDescriptor">(optional) Custom registration action, should return true to skip normal registration.</param>
        /// <example>
        /// <code><![CDATA[
        ///     // example of normal descriptor registration together with factory method registration for SomeService.
        ///     container.Populate(services, (r, service) => {
        ///         if (service.ServiceType == typeof(SomeService)) {
        ///             r.Register<SomeService>(Made.Of(() => CreateCustomService()), Reuse.Singleton);
        ///             return true;
        ///         };
        ///         return false; // fallback to normal registrations for the rest of the descriptors.
        ///     });
        /// ]]></code>
        /// </example>
        public static IContainer Populate(this IContainer container, IEnumerable<ServiceDescriptor> descriptors,
            Func<IRegistrator, ServiceDescriptor, bool> registerDescriptor = null)
        {
            if (registerDescriptor == null)
                foreach (var descriptor in descriptors)
                    container.RegisterDescriptor(descriptor);
            else
                foreach (var descriptor in descriptors)
                    if (!registerDescriptor(container, descriptor))
                        container.RegisterDescriptor(descriptor);
            return container;
        }

        /// <summary>Uses passed descriptor to register service in container:
        /// maps DI Lifetime to DryIoc Reuse,
        /// and DI registration type to corresponding DryIoc Register, RegisterDelegate or RegisterInstance.</summary>
        /// <param name="container">The container.</param>
        /// <param name="descriptor">Service descriptor.</param>
        public static void RegisterDescriptor(this IContainer container, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType != null)
            {
                var reuse = descriptor.Lifetime == ServiceLifetime.Singleton ? Reuse.Singleton
                    : descriptor.Lifetime == ServiceLifetime.Scoped ? Reuse.Scoped
                    : Reuse.Transient;

                // ensure eventing handlers are pulled in automagically.
                container.RegisterMany(
                    new [] {descriptor.ImplementationType},
                    reuse: reuse,
                    serviceTypeCondition: type => type == descriptor.ImplementationType || type == descriptor.ServiceType || typeof(IEventingHandler).IsAssignableFrom(type) || typeof(IJsonRpcHandler).IsAssignableFrom(type));
            }
            else if (descriptor.ImplementationFactory != null)
            {
                var reuse = descriptor.Lifetime == ServiceLifetime.Singleton ? Reuse.Singleton
                    : descriptor.Lifetime == ServiceLifetime.Scoped ? Reuse.Scoped
                    : Reuse.Transient;

                container.RegisterDelegate(true, descriptor.ServiceType,
                    descriptor.ImplementationFactory,
                    reuse);
            }
            else
            {
                // ensure eventing handlers are pulled in automagically.
                if (!(descriptor.ImplementationInstance is IEnumerable<object>) && (descriptor.ImplementationInstance is IEventingHandler || descriptor.ImplementationInstance is IJsonRpcHandler))
                {

                    container.RegisterInstanceMany(descriptor.ImplementationInstance);
                }
                else
                {
                    container.RegisterInstance(true, descriptor.ServiceType, descriptor.ImplementationInstance);
                }
            }
        }
    }
}
