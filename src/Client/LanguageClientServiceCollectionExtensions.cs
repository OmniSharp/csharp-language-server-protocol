using System;
using System.Linq;
using System.Reflection;
using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class LanguageClientServiceCollectionExtensions
    {
        internal static IContainer AddLanguageClientInternals(this IContainer container, LanguageClientOptions options, IServiceProvider outerServiceProvider)
        {
            container = container.AddLanguageProtocolInternals(options);

            container.RegisterInstance(options.ClientCapabilities);
            container.RegisterMany<LspClientReceiver>(
                reuse: Reuse.Singleton,
                nonPublicServiceTypes: true,
                ifAlreadyRegistered: IfAlreadyRegistered.Keep
            );
            if (options.OnUnhandledException != null)
            {
                container.RegisterInstance(options.OnUnhandledException);
            }
            else
            {
                container.RegisterDelegate(_ => new OnUnhandledExceptionHandler(e => _.GetRequiredService<LanguageClient>().Shutdown()), Reuse.Singleton);
            }

            container.RegisterMany<TextDocumentLanguageClient>(serviceTypeCondition: type => type.Name.Contains(nameof(TextDocumentLanguageClient)), reuse: Reuse.Singleton);
            container.RegisterMany<ClientLanguageClient>(serviceTypeCondition: type => type.Name.Contains(nameof(ClientLanguageClient)), reuse: Reuse.Singleton);
            container.RegisterMany<GeneralLanguageClient>(serviceTypeCondition: type => type.Name.Contains(nameof(GeneralLanguageClient)), reuse: Reuse.Singleton);
            container.RegisterMany<WindowLanguageClient>(serviceTypeCondition: type => type.Name.Contains(nameof(WindowLanguageClient)), reuse: Reuse.Singleton);
            container.RegisterMany<WorkspaceLanguageClient>(serviceTypeCondition: type => type.Name.Contains(nameof(WorkspaceLanguageClient)), reuse: Reuse.Singleton);
            container.RegisterInstance<IOptionsFactory<LanguageClientOptions>>(new ValueOptionsFactory<LanguageClientOptions>(options));

            container.RegisterMany<LanguageClient>(serviceTypeCondition: type => type == typeof(ILanguageClient) || type == typeof(LanguageClient), reuse: Reuse.Singleton);

            container.RegisterInstance(
                options.ClientInfo ?? new ClientInfo {
                    Name = Assembly.GetEntryAssembly()?.GetName().Name,
                    Version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                     ?.InformationalVersion ??
                              Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyVersionAttribute>()?.Version
                }
            );

            var providedConfiguration = options.Services.FirstOrDefault(z => z.ServiceType == typeof(IConfiguration) && z.ImplementationInstance is IConfiguration);
            container.RegisterDelegate<IConfiguration>(
                _ => {
                    var builder = new ConfigurationBuilder();
                    var outerConfiguration = outerServiceProvider?.GetService<IConfiguration>();
                    if (outerConfiguration != null)
                    {
                        builder.AddConfiguration(outerConfiguration, false);
                    }

                    if (providedConfiguration != null)
                    {
                        builder.AddConfiguration(providedConfiguration.ImplementationInstance as IConfiguration);
                    }

                    //var didChangeConfigurationProvider = _.GetRequiredService<DidChangeConfigurationProvider>();
                    return builder
                        //.AddConfiguration(didChangeConfigurationProvider)
                       .Build();
                },
                Reuse.Singleton
            );

            container.RegisterMany<LanguageClientWorkDoneManager>(Reuse.Singleton);
            container.RegisterMany<LanguageClientWorkspaceFoldersManager>(
                serviceTypeCondition: type => options.WorkspaceFolders || type != typeof(IJsonRpcHandler), reuse: Reuse.Singleton
            );
            container.RegisterMany<LanguageClientRegistrationManager>(
                serviceTypeCondition: type => options.DynamicRegistration || type != typeof(IJsonRpcHandler), reuse: Reuse.Singleton
            );

            return container;
        }

        public static IServiceCollection AddLanguageClient(this IServiceCollection services, Action<LanguageClientOptions> configureOptions = null) =>
            AddLanguageClient(services, Options.DefaultName, configureOptions);

        public static IServiceCollection AddLanguageClient(this IServiceCollection services, string name, Action<LanguageClientOptions> configureOptions = null)
        {
            // If we get called multiple times we're going to remove the default server
            // and force consumers to use the resolver.
            if (services.Any(d => d.ServiceType == typeof(LanguageClient) || d.ServiceType == typeof(ILanguageClient)))
            {
                services.RemoveAll<LanguageClient>();
                services.RemoveAll<ILanguageClient>();
                services.AddSingleton<ILanguageClient>(
                    _ =>
                        throw new NotSupportedException("LanguageClient has been registered multiple times, you must use LanguageClient instead")
                );
                services.AddSingleton<LanguageClient>(
                    _ =>
                        throw new NotSupportedException("LanguageClient has been registered multiple times, you must use LanguageClient instead")
                );
            }

            services
               .AddOptions()
               .AddLogging();
            services.TryAddSingleton<LanguageClientResolver>();
            services.TryAddSingleton(_ => _.GetRequiredService<LanguageClientResolver>().Get(name));
            services.TryAddSingleton<ILanguageClient>(_ => _.GetRequiredService<LanguageClientResolver>().Get(name));

            if (configureOptions != null)
            {
                services.Configure(name, configureOptions);
            }

            return services;
        }
    }
}
