using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace DryIoc;

/// <summary>Adapts DryIoc container to be used as MS.DI service provider, plus provides the helpers
/// to simplify work with adapted container.</summary>
internal static class DryIocAdapter
{
    /// <summary>Adapts passed <paramref name="container"/> to Microsoft.DependencyInjection conventions,
    /// registers DryIoc implementations of <see cref="IServiceProvider"/> and <see cref="IServiceScopeFactory"/>,
    /// and returns NEW container.
    /// </summary>
    /// <param name="container">Source container to adapt.</param>
    /// <param name="descriptors">(optional) Specify service descriptors or use <see cref="Populate"/> later.</param>
    /// <param name="registerDescriptor">(optional) Custom registration action, should return true to skip normal registration.</param>
    /// <param name="registrySharing">(optional) Use DryIoc <see cref="RegistrySharing"/> capability.</param>
    /// <example>
    /// <code><![CDATA[
    /// 
    ///     var container = new Container();
    ///
    ///     // you may register the services here:
    ///     container.Register<IMyService, MyService>(Reuse.Scoped)
    /// 
    ///     // applies the MS.DI rules and registers the infrastructure helpers and service collection to the container
    ///     var adaptedContainer = container.WithDependencyInjectionAdapter(services); 
    ///
    ///     // the container implements IServiceProvider
    ///     IServiceProvider serviceProvider = adaptedContainer;
    ///
    ///]]></code>
    /// </example>
    /// <remarks>You still need to Dispose adapted container at the end / application shutdown.</remarks>
    public static IContainer WithDependencyInjectionAdapter(
        this IContainer container,
        IEnumerable<ServiceDescriptor> descriptors = null,
        Func<IRegistrator, ServiceDescriptor, bool> registerDescriptor = null,
        RegistrySharing registrySharing = RegistrySharing.Share
    )
    {
        if (container.Rules != Rules.MicrosoftDependencyInjectionRules)
            container = container.With(
                container.Rules.WithMicrosoftDependencyInjectionRules(),
                container.ScopeContext, registrySharing, container.SingletonScope
            );

        var capabilities = new DryIocServiceProviderCapabilities(container);
        var singletons = container.SingletonScope;
#if NET6_0_OR_GREATER
            singletons.Use<IServiceProviderIsService>(capabilities);
#endif
        singletons.Use<ISupportRequiredService>(capabilities);
        singletons.UseFactory<IServiceScopeFactory>(r => new DryIocServiceScopeFactory(r));

        if (descriptors != null)
            container.Populate(descriptors, registerDescriptor);

        return container;
    }

    /// <summary>Sugar to create the DryIoc container and adapter populated with services</summary>
    public static IServiceProvider CreateServiceProvider(this IServiceCollection services) =>
        new Container(DryIoc.Rules.MicrosoftDependencyInjectionRules).WithDependencyInjectionAdapter(services);

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
    /// public class ExampleCompositionRoot
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
    public static IContainer Populate(
        this IContainer container, IEnumerable<ServiceDescriptor> descriptors,
        Func<IRegistrator, ServiceDescriptor, bool> registerDescriptor = null
    )
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

    /// <summary>Converts the MS.DI ServiceLifetime into the corresponding `DryIoc.IReuse`</summary>
    [MethodImpl((MethodImplOptions)256)]
    public static IReuse ToReuse(this ServiceLifetime lifetime) =>
        lifetime == ServiceLifetime.Singleton ? Reuse.Singleton :
        lifetime == ServiceLifetime.Scoped ? Reuse.ScopedOrSingleton : // see, that we have Reuse.ScopedOrSingleton here instead of Reuse.Scoped
        Reuse.Transient;

    /// <summary>Unpacks the service descriptor to register the service in DryIoc container</summary>
    public static void RegisterDescriptor(this IContainer container, ServiceDescriptor descriptor)
    {
        var serviceType = descriptor.ServiceType;
        var implType = descriptor.ImplementationType;
        if (implType != null)
        {
            // ensure eventing handlers are pulled in automagically.
            container.RegisterMany(
                new [] {descriptor.ImplementationType},
                reuse: descriptor.Lifetime.ToReuse(),
                serviceTypeCondition: type => type == descriptor.ImplementationType || type == descriptor.ServiceType || typeof(IEventingHandler).IsAssignableFrom(type) || typeof(IJsonRpcHandler).IsAssignableFrom(type),
                nonPublicServiceTypes: true
            );
        }
        else if (descriptor.ImplementationFactory != null)
        {
            container.Register(
                DelegateFactory.Of(descriptor.ImplementationFactory.ToFactoryDelegate, descriptor.Lifetime.ToReuse()), serviceType,
                null, null, isStaticallyChecked: true
            );
        }
        else
        {
            // ensure eventing handlers are pulled in automagically.
            if (descriptor.ImplementationInstance is not IEnumerable<object>
             && descriptor.ImplementationInstance is IEventingHandler or IJsonRpcHandler)
            {
                var instance = descriptor.ImplementationInstance;
                container.RegisterInstanceMany(instance, nonPublicServiceTypes: true);
                container.TrackDisposable(instance);
            }
            else
            {
                var instance = descriptor.ImplementationInstance;
                container.Register(
                    InstanceFactory.Of(instance), serviceType,
                    null, null, isStaticallyChecked: true
                );
                container.TrackDisposable(instance);
            }
        }
    }
}
