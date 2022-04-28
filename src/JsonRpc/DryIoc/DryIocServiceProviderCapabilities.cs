using Microsoft.Extensions.DependencyInjection;

namespace DryIoc;

/// <summary>Wrapper of DryIoc `IsRegistered` and `Resolve` throwing the exception on unresolved type capabilities.</summary>
internal sealed class DryIocServiceProviderCapabilities :
#if NET6_0_OR_GREATER
        IServiceProviderIsService,
#endif
    ISupportRequiredService
{
    private readonly IContainer _container;

    /// <summary>Statefully wraps the passed <paramref name="container"/></summary>
    public DryIocServiceProviderCapabilities(IContainer container) => _container = container;

    /// <inheritdoc />
    public bool IsService(Type serviceType)
    {
        // I am not fully comprehend but MS.DI considers asking for the open-generic type even if it is registered to return `false`
        // Probably mixing here the fact that open type cannot be instantiated without providing the concrete type argument.
        // But I think it is conflating two things and making the reasoning harder.
        if (serviceType.IsGenericTypeDefinition)
            return false;

        if (
#if NET6_0_OR_GREATER
                serviceType == typeof(IServiceProviderIsService) ||
#endif
            serviceType == typeof(ISupportRequiredService) ||
            serviceType == typeof(IServiceScopeFactory))
            return true;

        if (_container.IsRegistered(serviceType))
            return true;

        if (serviceType.IsGenericType &&
            _container.IsRegistered(serviceType.GetGenericTypeDefinition()))
            return true;

        return _container.IsRegistered(serviceType, factoryType: FactoryType.Wrapper);
    }

    /// <inheritdoc />
    public object GetRequiredService(Type serviceType) => _container.Resolve(serviceType);
}