using System;
using System.Linq;
using System.Reflection;
using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using OmniSharp.Extensions.LanguageServer.Server.Configuration;
using OmniSharp.Extensions.LanguageServer.Server.Logging;
using OmniSharp.Extensions.LanguageServer.Server.Matchers;
using OmniSharp.Extensions.LanguageServer.Server.Pipelines;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class LanguageServerServiceCollectionExtensions
    {
        internal static IContainer AddLanguageServerInternals(this IContainer container, LanguageServerOptions options, IServiceProvider outerServiceProvider)
        {
            if (options.Receiver == null)
            {
                throw new ArgumentException("Receiver is missing!", nameof(options));
            }

            container = container.AddLanguageProtocolInternals(options);

            container.RegisterInstanceMany(options.Receiver);
            if (options.OnUnhandledException != null)
            {
                container.RegisterInstance(options.OnUnhandledException);
            }
            else
            {
                container.RegisterDelegate(_ => new OnUnhandledExceptionHandler(e => { _.GetRequiredService<LanguageServer>().ForcefulShutdown(); }), Reuse.Singleton);
            }

            container.RegisterMany<TextDocumentLanguageServer>(serviceTypeCondition: type => type.Name.Contains(nameof(TextDocumentLanguageServer)), reuse: Reuse.Singleton);
            container.RegisterMany<ClientLanguageServer>(serviceTypeCondition: type => type.Name.Contains(nameof(ClientLanguageServer)), reuse: Reuse.Singleton);
            container.RegisterMany<GeneralLanguageServer>(serviceTypeCondition: type => type.Name.Contains(nameof(GeneralLanguageServer)), reuse: Reuse.Singleton);
            container.RegisterMany<WindowLanguageServer>(serviceTypeCondition: type => type.Name.Contains(nameof(WindowLanguageServer)), reuse: Reuse.Singleton);
            container.RegisterMany<WorkspaceLanguageServer>(serviceTypeCondition: type => type.Name.Contains(nameof(WorkspaceLanguageServer)), reuse: Reuse.Singleton);
            container.RegisterInstance<IOptionsFactory<LanguageServerOptions>>(new ValueOptionsFactory<LanguageServerOptions>(options));

            container.RegisterMany<LanguageServer>(serviceTypeCondition: type => type == typeof(ILanguageServer) || type == typeof(LanguageServer), reuse: Reuse.Singleton);

            container.RegisterMany<DidChangeConfigurationProvider>(
                made: Parameters.Of
                                .Type<Action<IConfigurationBuilder>>(defaultValue: options.ConfigurationBuilderAction),
                reuse: Reuse.Singleton
            );

            var providedConfiguration = options.Services.FirstOrDefault(z => z.ServiceType == typeof(IConfiguration) && z.ImplementationInstance is IConfiguration);
            container.RegisterDelegate<IConfiguration>(
                _ => {
                    var builder = new ConfigurationBuilder();
                    var didChangeConfigurationProvider = _.GetRequiredService<DidChangeConfigurationProvider>();
                    var outerConfiguration = outerServiceProvider?.GetService<IConfiguration>();
                    if (outerConfiguration != null)
                    {
                        builder.AddConfiguration(outerConfiguration, false);
                    }

                    if (providedConfiguration != null)
                    {
                        builder.AddConfiguration(providedConfiguration.ImplementationInstance as IConfiguration);
                    }

                    return builder.AddConfiguration(didChangeConfigurationProvider).Build();
                },
                Reuse.Singleton
            );

            container.RegisterMany<LanguageServerLoggerFilterOptions>(serviceTypeCondition: type => type.IsInterface, reuse: Reuse.Singleton);
            container.RegisterInstance(
                options.ServerInfo ?? new ServerInfo {
                    Name = Assembly.GetEntryAssembly()?.GetName().Name,
                    Version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                     ?.InformationalVersion ??
                              Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyVersionAttribute>()?.Version
                }
            );

            container.RegisterMany<TextDocumentMatcher>(Reuse.Singleton);
            container.RegisterMany<ExecuteCommandMatcher>(Reuse.Singleton);
            container.RegisterMany<ResolveCommandMatcher>(Reuse.Singleton);
            container.RegisterMany(new[] { typeof(ResolveCommandPipeline<,>) });
            container.RegisterMany<LanguageServerWorkDoneManager>(Reuse.Singleton);
            container.RegisterMany<LanguageServerWorkspaceFolderManager>(Reuse.Singleton);

            return container;
        }

        public static IServiceCollection AddLanguageServer(this IServiceCollection services, Action<LanguageServerOptions> configureOptions = null) =>
            AddLanguageServer(services, Options.DefaultName, configureOptions);

        public static IServiceCollection AddLanguageServer(this IServiceCollection services, string name, Action<LanguageServerOptions> configureOptions = null)
        {
            // If we get called multiple times we're going to remove the default server
            // and force consumers to use the resolver.
            if (services.Any(d => d.ServiceType == typeof(LanguageServer) || d.ServiceType == typeof(ILanguageServer)))
            {
                services.RemoveAll<LanguageServer>();
                services.RemoveAll<ILanguageServer>();
                services.AddSingleton<ILanguageServer>(
                    _ =>
                        throw new NotSupportedException("LanguageServer has been registered multiple times, you must use LanguageServer instead")
                );
                services.AddSingleton<LanguageServer>(
                    _ =>
                        throw new NotSupportedException("LanguageServer has been registered multiple times, you must use LanguageServer instead")
                );
            }

            services
               .AddOptions()
               .AddLogging();
            services.TryAddSingleton<LanguageServerResolver>();
            services.TryAddSingleton(_ => _.GetRequiredService<LanguageServerResolver>().Get(name));
            services.TryAddSingleton<ILanguageServer>(_ => _.GetRequiredService<LanguageServerResolver>().Get(name));

            if (configureOptions != null)
            {
                services.Configure(name, configureOptions);
            }

            return services;
        }
    }
}
