using System;
using DryIoc;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable once CheckNamespace

namespace DryIoc
{
    /// <summary>This DryIoc is supposed to be used with `IHostBuilder` like this:
    /// <code><![CDATA[
    /// internal class Program
    /// {
    ///     public static async Task Main(string[] args) =>
    ///         await CreateHostBuilder(args).Build().RunAsync();
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
    ///
    /// DON'T forget to add `services.AddControllers().AddControllersAsServices` in `Startup.ConfigureServices`
    /// in order to access DryIoc diagnostics for controllers, property-injection, etc.
    ///
    /// That's probably ALL YOU NEED to do.
    /// </summary>
    internal class DryIocServiceProviderFactory : IServiceProviderFactory<IContainer>
    {
        private readonly IContainer _container;
        private readonly Func<IRegistrator, ServiceDescriptor, bool> _registerDescriptor;

        /// Some options to push to `.WithDependencyInjectionAdapter(...)`
        public DryIocServiceProviderFactory(
            IContainer container = null,
            Func<IRegistrator, ServiceDescriptor, bool> registerDescriptor = null)
        {
            _container = container; // we won't initialize the container here because it is logically expected to be done in `CreateBuilder`
            _registerDescriptor = registerDescriptor;
        }

        /// <inheritdoc />
        public IContainer CreateBuilder(IServiceCollection services) =>
            (_container ?? new Container()).WithDependencyInjectionAdapter(services, _registerDescriptor);

        /// <inheritdoc />
        public IServiceProvider CreateServiceProvider(IContainer container) =>
            container.BuildServiceProvider();
    }
}
