﻿using System;
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

#pragma warning disable CS0618

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class LanguageServerServiceCollectionExtensions
    {
        internal static IContainer AddLanguageServerInternals(this IContainer container, LanguageServerOptions options, IServiceProvider? outerServiceProvider)
        {
            options.WithAssemblies(typeof(LanguageServerServiceCollectionExtensions).Assembly, typeof(LspRequestRouter).Assembly);
            container = container.AddLanguageProtocolInternals(options);
            container.RegisterMany<LspServerReceiver>(
                reuse: Reuse.Singleton,
                nonPublicServiceTypes: true,
                ifAlreadyRegistered: IfAlreadyRegistered.Keep
            );
            container.RegisterMany<LspServerOutputFilter>(Reuse.Singleton, nonPublicServiceTypes: true);

            if (options.OnUnhandledException != null)
            {
                container.RegisterInstance(options.OnUnhandledException);
            }
            else
            {
                container.RegisterDelegate(_ => new OnUnhandledExceptionHandler(e => { _.GetRequiredService<LanguageServer>().ForcefulShutdown(); }), Reuse.Singleton);
            }

            container.RegisterMany<TextDocumentLanguageServer>(serviceTypeCondition: type => type.Name.Contains(nameof(TextDocumentLanguageServer)), reuse: Reuse.Singleton);
            container.RegisterMany<NotebookDocumentLanguageServer>(serviceTypeCondition: type => type.Name.Contains(nameof(NotebookDocumentLanguageServer)), reuse: Reuse.Singleton);
            container.RegisterMany<ClientLanguageServer>(serviceTypeCondition: type => type.Name.Contains(nameof(ClientLanguageServer)), reuse: Reuse.Singleton);
            container.RegisterMany<GeneralLanguageServer>(serviceTypeCondition: type => type.Name.Contains(nameof(GeneralLanguageServer)), reuse: Reuse.Singleton);
            container.RegisterMany<WindowLanguageServer>(serviceTypeCondition: type => type.Name.Contains(nameof(WindowLanguageServer)), reuse: Reuse.Singleton);
            container.RegisterMany<WorkspaceLanguageServer>(serviceTypeCondition: type => type.Name.Contains(nameof(WorkspaceLanguageServer)), reuse: Reuse.Singleton);
            container.RegisterMany<DefaultLanguageServerFacade>(
                serviceTypeCondition: type => type.IsClass || !type.Name.Contains("Proxy") && typeof(DefaultLanguageServerFacade).GetInterfaces()
                   .Except(typeof(DefaultLanguageServerFacade).BaseType!.GetInterfaces()).Any(z => type == z),
                reuse: Reuse.Singleton
            );
            container.RegisterInstance<IOptionsFactory<LanguageServerOptions>>(new ValueOptionsFactory<LanguageServerOptions>(options));

            container.RegisterMany<LanguageServer>(
                serviceTypeCondition: type => type == typeof(ILanguageServer) || type == typeof(LanguageServer),
                reuse: Reuse.Singleton,
                setup: Setup.With(condition: req => req.IsResolutionRoot || req.Container.Resolve<IInsanceHasStarted>().Started)
            );

            container.RegisterMany<DidChangeConfigurationProvider>(
                made: Parameters.Of
                                .Type<Action<IConfigurationBuilder>>(defaultValue: options.ConfigurationBuilderAction),
                reuse: Reuse.Singleton
            );
            container.RegisterInitializer<ILanguageServerConfiguration>(
                (provider, context) => {
                    provider.AddConfigurationItems(context.ResolveMany<ConfigurationItem>());
                }
            );
            container.RegisterMany<ConfigurationConverter>(nonPublicServiceTypes: true, reuse: Reuse.Singleton);


            var providedConfiguration = options.Services.FirstOrDefault(z => z.ServiceType == typeof(IConfiguration) && z.ImplementationInstance is IConfiguration);
            container.RegisterDelegate<IConfiguration>(
                _ => {
                    var builder = options.ConfigurationBuilder;
                    var didChangeConfigurationProvider = _.GetRequiredService<ILanguageServerConfiguration>();
                    var outerConfiguration = outerServiceProvider?.GetService<IConfiguration>();
                    if (outerConfiguration != null)
                    {
                        builder.CustomAddConfiguration(outerConfiguration, false);
                    }

                    if (providedConfiguration != null)
                    {
                        builder.CustomAddConfiguration(( providedConfiguration.ImplementationInstance as IConfiguration )!);
                    }

                    return builder.CustomAddConfiguration(didChangeConfigurationProvider).Build();
                },
                Reuse.Singleton
            );

            container.RegisterMany<LanguageServerLoggingManager>(nonPublicServiceTypes: true, reuse: Reuse.Singleton);
            container.RegisterInstance(
                options.ServerInfo ?? new ServerInfo {
                    Name = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty,
                    Version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                     ?.InformationalVersion ??
                              Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyVersionAttribute>()?.Version
                }
            );

            container.RegisterMany<TextDocumentMatcher>(Reuse.Singleton);
            container.RegisterMany<NotebookDocumentMatcher>(Reuse.Singleton);
            container.RegisterMany<ExecuteCommandMatcher>(Reuse.Singleton);
            container.RegisterMany<ResolveCommandMatcher>(Reuse.Singleton);
            container.RegisterMany(new[] { typeof(ResolveCommandPipeline<,>) });
            container.RegisterMany(new[] { typeof(SemanticTokensDeltaPipeline<,>) });
            container.RegisterMany<LanguageServerWorkDoneManager>(Reuse.Singleton);
            container.RegisterMany<LanguageServerWorkspaceFolderManager>(reuse: Reuse.Singleton);

            return container;
        }

        public static IServiceCollection AddLanguageServer(this IServiceCollection services, Action<LanguageServerOptions>? configureOptions = null) =>
            AddLanguageServer(services, Options.DefaultName, configureOptions);

        public static IServiceCollection AddLanguageServer(this IServiceCollection services, string name, Action<LanguageServerOptions>? configureOptions = null)
        {
            // If we get called multiple times we're going to remove the default server
            // and force consumers to use the resolver.
            if (services.Any(d => d.ServiceType == typeof(LanguageServer) || d.ServiceType == typeof(ILanguageServer)))
            {
                services.RemoveAll<LanguageServer>();
                services.RemoveAll<ILanguageServer>();
                services.RemoveAll<ILanguageServerFacade>();
                services.AddSingleton<ILanguageServer>(
                    _ =>
                        throw new NotSupportedException("LanguageServer has been registered multiple times, you must use LanguageServerResolver instead")
                );
                services.AddSingleton<ILanguageServerFacade>(
                    _ =>
                        throw new NotSupportedException("LanguageServer has been registered multiple times, you must use LanguageServerResolver instead")
                );
                services.AddSingleton<LanguageServer>(
                    _ =>
                        throw new NotSupportedException("LanguageServer has been registered multiple times, you must use LanguageServerResolver instead")
                );
            }

            services
               .AddOptions()
               .AddLogging();
            services.TryAddSingleton<LanguageServerResolver>();
            services.TryAddSingleton(_ => _.GetRequiredService<LanguageServerResolver>().Get(name));
            services.TryAddSingleton<ILanguageServer>(_ => _.GetRequiredService<LanguageServerResolver>().Get(name));
            services.TryAddSingleton<ILanguageServerFacade>(_ => _.GetRequiredService<LanguageServerResolver>().Get(name));

            if (configureOptions != null)
            {
                services.Configure(name, configureOptions);
            }

            return services;
        }
    }
}
