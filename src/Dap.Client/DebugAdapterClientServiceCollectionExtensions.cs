using System;
using System.Linq;
using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OmniSharp.Extensions.DebugAdapter.Client.Configuration;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Shared;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Client
{
    public static class DebugAdapterClientServiceCollectionExtensions
    {
        internal static IContainer AddDebugAdapterClientInternals(this IContainer container, DebugAdapterClientOptions options, IServiceProvider? outerServiceProvider)
        {
            container = container.AddDebugAdapterProtocolInternals(options);

            if (options.OnUnhandledException != null)
            {
                container.RegisterInstance(options.OnUnhandledException);
            }
            else
            {
                container.RegisterDelegate(_ => new OnUnhandledExceptionHandler(e => { }), Reuse.Singleton);
            }

            container.RegisterInstance<IOptionsFactory<DebugAdapterClientOptions>>(new ValueOptionsFactory<DebugAdapterClientOptions>(options));

            container.RegisterInstance(
                new InitializeRequestArguments {
                    Locale = options.Locale,
                    AdapterId = options.AdapterId!,
                    ClientId = options.ClientId,
                    ClientName = options.ClientName,
                    PathFormat = options.PathFormat,
                    ColumnsStartAt1 = options.ColumnsStartAt1 ?? false,
                    LinesStartAt1 = options.LinesStartAt1 ?? false,
                    SupportsMemoryReferences = options.SupportsMemoryReferences ?? false,
                    SupportsProgressReporting = options.SupportsProgressReporting ?? false,
                    SupportsVariablePaging = options.SupportsVariablePaging ?? false,
                    SupportsVariableType = options.SupportsVariableType ?? false,
                    SupportsRunInTerminalRequest = options.SupportsRunInTerminalRequest ?? false,
                }
            );
            container.RegisterInstance(options.RequestProcessIdentifier);

            container.RegisterMany<DebugAdapterClientProgressManager>(nonPublicServiceTypes: true, reuse: Reuse.Singleton);
            container.RegisterMany<DebugAdapterClient>(
                serviceTypeCondition: type => type == typeof(IDebugAdapterClient) || type == typeof(DebugAdapterClient),
                reuse: Reuse.Singleton,
                setup: Setup.With(condition: req => req.IsResolutionRoot || req.Container.Resolve<IInsanceHasStarted>().Started)
            );
            container.RegisterMany<DefaultDebugAdapterClientFacade>(
                serviceTypeCondition: type => type.IsClass || !type.Name.Contains("Proxy") && typeof(DefaultDebugAdapterClientFacade).GetInterfaces().Except(typeof(DefaultDebugAdapterClientFacade).BaseType!.GetInterfaces()).Any(z => type == z),
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
                        builder.CustomAddConfiguration(providedConfiguration.ImplementationInstance as IConfiguration);
                    }

                    return builder.Build();
                },
                Reuse.Singleton
            );

            return container;
        }

        public static IServiceCollection AddDebugAdapterClient(this IServiceCollection services, Action<DebugAdapterClientOptions>? configureOptions = null) =>
            AddDebugAdapterClient(services, Options.DefaultName, configureOptions);

        public static IServiceCollection AddDebugAdapterClient(this IServiceCollection services, string name, Action<DebugAdapterClientOptions>? configureOptions = null)
        {
            // If we get called multiple times we're going to remove the default server
            // and force consumers to use the resolver.
            if (services.Any(d => d.ServiceType == typeof(DebugAdapterClient) || d.ServiceType == typeof(IDebugAdapterClient)))
            {
                services.RemoveAll<DebugAdapterClient>();
                services.RemoveAll<IDebugAdapterClient>();
                services.AddSingleton<IDebugAdapterClient>(
                    _ =>
                        throw new NotSupportedException("DebugAdapterClient has been registered multiple times, you must use DebugAdapterClient instead")
                );
                services.AddSingleton<DebugAdapterClient>(
                    _ =>
                        throw new NotSupportedException("DebugAdapterClient has been registered multiple times, you must use DebugAdapterClient instead")
                );
            }

            services
               .AddOptions()
               .AddLogging();
            services.TryAddSingleton<DebugAdapterClientResolver>();
            services.TryAddSingleton(_ => _.GetRequiredService<DebugAdapterClientResolver>().Get(name));
            services.TryAddSingleton<IDebugAdapterClient>(_ => _.GetRequiredService<DebugAdapterClientResolver>().Get(name));

            if (configureOptions != null)
            {
                services.Configure(name, configureOptions);
            }

            return services;
        }
    }
}
