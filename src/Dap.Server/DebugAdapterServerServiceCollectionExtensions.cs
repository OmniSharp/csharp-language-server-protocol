using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.DebugAdapter.Server.Configuration;
using OmniSharp.Extensions.DebugAdapter.Shared;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    public static class DebugAdapterServerServiceCollectionExtensions
    {
        internal static IContainer AddDebugAdapterServerInternals(this IContainer container, DebugAdapterServerOptions options, IServiceProvider? outerServiceProvider)
        {
            container = container.AddDebugAdapterProtocolInternals(options);

            if (options.OnUnhandledException != null)
            {
                container.RegisterInstance(options.OnUnhandledException);
            }
            else
            {
                container.RegisterDelegate(_ => new OnUnhandledExceptionHandler(_ => { }), Reuse.Singleton);
            }

            container.RegisterInstance<IOptionsFactory<DebugAdapterServerOptions>>(new ValueOptionsFactory<DebugAdapterServerOptions>(options));

            container.RegisterInstance(options.Capabilities);
            container.RegisterInstance(options.RequestProcessIdentifier);

            container.RegisterMany<DebugAdapterServerProgressManager>(nonPublicServiceTypes: true, reuse: Reuse.Singleton);
            container.RegisterMany<DebugAdapterServer>(
                serviceTypeCondition: type => type == typeof(IDebugAdapterServer) || type == typeof(DebugAdapterServer),
                reuse: Reuse.Singleton,
                setup: Setup.With(condition: req => req.IsResolutionRoot || req.Container.Resolve<IInsanceHasStarted>().Started)
            );
            container.RegisterMany<DefaultDebugAdapterServerFacade>(
                serviceTypeCondition: type => type.IsClass || !type.Name.Contains("Proxy") && typeof(DefaultDebugAdapterServerFacade).GetInterfaces().Except(typeof(DefaultDebugAdapterServerFacade).BaseType!.GetInterfaces()).Any(z => type == z),
                reuse: Reuse.Singleton
            );

            // container.
            var providedConfiguration = options.Services.FirstOrDefault(z => z.ServiceType == typeof(IConfiguration) && z.ImplementationInstance is IConfiguration);
            container.RegisterDelegate<IConfiguration>(
                _ => {
                    var builder = new ConfigurationBuilder();
                    if (outerServiceProvider != null)
                    {
                        var outerConfiguration = outerServiceProvider.GetService<IConfiguration>();
                        if (outerConfiguration != null)
                        {
                            builder.CustomAddConfiguration(outerConfiguration, false);
                        }
                    }

                    if (providedConfiguration != null)
                    {
                        builder.CustomAddConfiguration((providedConfiguration.ImplementationInstance as IConfiguration)!);
                    }

                    return builder.Build();
                },
                Reuse.Singleton
            );

            return container;
        }

        public static IServiceCollection AddDebugAdapterServer(this IServiceCollection services, Action<DebugAdapterServerOptions>? configureOptions = null) =>
            AddDebugAdapterServer(services, Options.DefaultName, configureOptions);

        public static IServiceCollection AddDebugAdapterServer(this IServiceCollection services, string name, Action<DebugAdapterServerOptions>? configureOptions = null)
        {
            // If we get called multiple times we're going to remove the default server
            // and force consumers to use the resolver.
            if (services.Any(d => d.ServiceType == typeof(DebugAdapterServer) || d.ServiceType == typeof(IDebugAdapterServer)))
            {
                services.RemoveAll<DebugAdapterServer>();
                services.RemoveAll<IDebugAdapterServer>();
                services.RemoveAll<IDebugAdapterServerFacade>();
                services.AddSingleton<IDebugAdapterServer>(
                    _ =>
                        throw new NotSupportedException("DebugAdapterServer has been registered multiple times, you must use DebugAdapterServerResolver instead")
                );
                services.AddSingleton<IDebugAdapterServerFacade>(
                    _ =>
                        throw new NotSupportedException("DebugAdapterServer has been registered multiple times, you must use DebugAdapterServerResolver instead")
                );
                services.AddSingleton<DebugAdapterServer>(
                    _ =>
                        throw new NotSupportedException("DebugAdapterServer has been registered multiple times, you must use DebugAdapterServerResolver instead")
                );
            }

            services
               .AddOptions()
               .AddLogging();
            services.TryAddSingleton<DebugAdapterServerResolver>();
            services.TryAddSingleton(_ => _.GetRequiredService<DebugAdapterServerResolver>().Get(name));
            services.TryAddSingleton<IDebugAdapterServer>(_ => _.GetRequiredService<DebugAdapterServerResolver>().Get(name));
            services.TryAddSingleton<IDebugAdapterServerFacade>(_ => _.GetRequiredService<DebugAdapterServerResolver>().Get(name));

            if (configureOptions != null)
            {
                services.Configure(name, configureOptions);
            }

            return services;
        }
    }
}
