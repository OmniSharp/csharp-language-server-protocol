using Microsoft.Extensions.DependencyInjection;

namespace DryIoc;

/// <summary>
/// This DryIoc is supposed to be used with generic `IHostBuilder` like this:
/// 
/// <code><![CDATA[
/// public class Program
/// {
///     public static async Task Main(string[] args) => 
///         await CreateHostBuilder(args).Build().RunAsync();
/// 
///     Rules WithMyRules(Rules currentRules) => currentRules;
///
///     public static IHostBuilder CreateHostBuilder(string[] args) =>
///         Host.CreateDefaultBuilder(args)
///             .UseServiceProviderFactory(new DryIocServiceProviderFactory(new Container(rules => WithMyRules(rules))))
///             .ConfigureWebHostDefaults(webBuilder =>
///             {
///                 webBuilder.UseStartup<Startup>();
///             });
/// }
/// ]]></code>
/// 
/// Then register your services in `Startup.ConfigureContainer`.
/// 
/// DON'T try to change the container rules there - they will be lost, 
/// instead pass the pre-configured container to `DryIocServiceProviderFactory` as in example above.
/// By default container will use <see href="DryIoc.Rules.MicrosoftDependencyInjectionRules" />
/// 
/// DON'T forget to add `services.AddControllers().AddControllersAsServices()` in `Startup.ConfigureServices` 
/// in order to access DryIoc diagnostics for controllers, property-injection, etc.
/// 
/// </summary>
internal class DryIocServiceProviderFactory : IServiceProviderFactory<IContainer>
{
    private readonly IContainer _container;
    private readonly Func<IRegistrator, ServiceDescriptor, bool> _registerDescriptor;
    private readonly RegistrySharing _registrySharing;

    /// <summary>
    /// We won't initialize the container here because it is logically expected to be done in `CreateBuilder`,
    /// so the factory constructor is just saving some options to use later.
    /// </summary>
    public DryIocServiceProviderFactory(
        IContainer container = null,
        Func<IRegistrator, ServiceDescriptor, bool> registerDescriptor = null
    ) :
        this(container, RegistrySharing.CloneAndDropCache, registerDescriptor)
    {
    }

    /// <summary>
    /// `container` is the existing container which will be cloned with the MS.DI rules and its cache will be dropped,
    /// unless the `registrySharing` is set to the `RegistrySharing.Share` or to `RegistrySharing.CloneButKeepCache`.
    /// `registerDescriptor` is the custom service descriptor handler.
    /// </summary>
    public DryIocServiceProviderFactory(
        IContainer container, RegistrySharing registrySharing,
        Func<IRegistrator, ServiceDescriptor, bool> registerDescriptor = null
    )
    {
        _container = container;
        _registrySharing = registrySharing;
        _registerDescriptor = registerDescriptor;
    }

    /// <inheritdoc />
    public IContainer CreateBuilder(IServiceCollection services) =>
        ( _container ?? new Container(Rules.MicrosoftDependencyInjectionRules) )
       .WithDependencyInjectionAdapter(services, _registerDescriptor, _registrySharing);

    /// <inheritdoc />
    public IServiceProvider CreateServiceProvider(IContainer container) =>
        container.BuildServiceProvider();
}