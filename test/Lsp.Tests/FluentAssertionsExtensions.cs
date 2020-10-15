using System;
using FluentAssertions.Equivalency;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace Lsp.Tests
{
    public static class FluentAssertionsExtensions
    {

        /// <summary>
        /// Registers a injected configuration service which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services)
            where TOptions : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return Configure<TOptions>(services, null);
        }

        /// <summary>
        /// Registers a injected configuration service which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="sectionName">The name of the options instance.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string? sectionName)
            where TOptions : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(
                _ => new ConfigurationChangeTokenSource<TOptions>(
                    Options.DefaultName,
                    sectionName == null ? _.GetRequiredService<IConfiguration>() : _.GetRequiredService<IConfiguration>().GetSection(sectionName)
                )
            );
            return services.AddSingleton<IConfigureOptions<TOptions>>(
                _ => new NamedConfigureFromConfigurationOptions<TOptions>(
                    Options.DefaultName,
                    sectionName == null ? _.GetRequiredService<IConfiguration>() : _.GetRequiredService<IConfiguration>().GetSection(sectionName)
                )
            );
        }

        public static EquivalencyAssertionOptions<T> ConfigureForSupports<T>(this EquivalencyAssertionOptions<T> options, ILogger? logger = null) =>
            options
               .WithTracing(new TraceWriter(logger ?? NullLogger.Instance))
               .ComparingByMembers<Supports<bool>>()
               .ComparingByMembers<Supports<SynchronizationCapability>>()
               .ComparingByMembers<Supports<CompletionCapability>>()
               .ComparingByMembers<Supports<HoverCapability>>()
               .ComparingByMembers<Supports<SignatureHelpCapability>>()
               .ComparingByMembers<Supports<ReferenceCapability>>()
               .ComparingByMembers<Supports<DeclarationCapability>>()
               .ComparingByMembers<Supports<DocumentHighlightCapability>>()
               .ComparingByMembers<Supports<DocumentSymbolCapability>>()
               .ComparingByMembers<Supports<DocumentFormattingCapability>>()
               .ComparingByMembers<Supports<DocumentRangeFormattingCapability>>()
               .ComparingByMembers<Supports<DocumentOnTypeFormattingCapability>>()
               .ComparingByMembers<Supports<DefinitionCapability>>()
               .ComparingByMembers<Supports<CodeActionCapability>>()
               .ComparingByMembers<Supports<CodeLensCapability>>()
               .ComparingByMembers<Supports<DocumentLinkCapability>>()
               .ComparingByMembers<Supports<RenameCapability>>()
               .ComparingByMembers<Supports<TypeDefinitionCapability>>()
               .ComparingByMembers<Supports<ImplementationCapability>>()
               .ComparingByMembers<Supports<ColorProviderCapability>>()
               .ComparingByMembers<Supports<PublishDiagnosticsCapability>>()
               .ComparingByMembers<Supports<WorkspaceEditCapability>>()
               .ComparingByMembers<Supports<DidChangeConfigurationCapability>>()
               .ComparingByMembers<Supports<DidChangeWatchedFilesCapability>>()
               .ComparingByMembers<Supports<WorkspaceSymbolCapability>>()
               .ComparingByMembers<Supports<ExecuteCommandCapability>>()
               .ComparingByMembers<Supports<FoldingRangeCapability>>()
               .ComparingByMembers<Supports<SelectionRangeCapability>>()
               .ComparingByMembers<Supports<TagSupportCapabilityOptions>>()
               .ComparingByMembers<Supports<CompletionItemTagSupportCapabilityOptions>>();
    }
}
