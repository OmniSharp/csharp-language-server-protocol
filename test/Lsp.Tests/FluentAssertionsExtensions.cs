using FluentAssertions.Equivalency;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace Lsp.Tests
{
    public static class FluentAssertionsExtensions
    {
        public static EquivalencyAssertionOptions<T> ConfigureForSupports<T>(this EquivalencyAssertionOptions<T> options, ILogger logger)
        {
            return options
                .WithTracing(new TraceWriter(logger))
                .ComparingByMembers<Supports<bool>>()
                .ComparingByMembers<Supports<TextDocumentSyncClientCapabilities>>()
                .ComparingByMembers<Supports<CompletionClientCapabilities>>()
                .ComparingByMembers<Supports<HoverClientCapabilities>>()
                .ComparingByMembers<Supports<SignatureHelpClientCapabilities>>()
                .ComparingByMembers<Supports<ReferenceClientCapabilities>>()
                .ComparingByMembers<Supports<DeclarationClientCapabilities>>()
                .ComparingByMembers<Supports<DocumentHighlightClientCapabilities>>()
                .ComparingByMembers<Supports<DocumentSymbolClientCapabilities>>()
                .ComparingByMembers<Supports<DocumentFormattingClientCapabilities>>()
                .ComparingByMembers<Supports<DocumentRangeFormattingClientCapabilities>>()
                .ComparingByMembers<Supports<DocumentOnTypeFormattingClientCapabilities>>()
                .ComparingByMembers<Supports<DefinitionClientCapabilities>>()
                .ComparingByMembers<Supports<CodeActionClientCapabilities>>()
                .ComparingByMembers<Supports<CodeLensClientCapabilities>>()
                .ComparingByMembers<Supports<DocumentLinkClientCapabilities>>()
                .ComparingByMembers<Supports<RenameClientCapabilities>>()
                .ComparingByMembers<Supports<TypeDefinitionClientCapabilities>>()
                .ComparingByMembers<Supports<ImplementationClientCapabilities>>()
                .ComparingByMembers<Supports<ColorProviderClientCapabilities>>()
                .ComparingByMembers<Supports<PublishDiagnosticsClientCapabilities>>()
                .ComparingByMembers<Supports<WorkspaceEditClientCapabilities>>()
                .ComparingByMembers<Supports<DidChangeConfigurationClientCapabilities>>()
                .ComparingByMembers<Supports<DidChangeWatchedFilesClientCapabilities>>()
                .ComparingByMembers<Supports<WorkspaceSymbolClientCapabilities>>()
                .ComparingByMembers<Supports<ExecuteCommandClientCapabilities>>()
                .ComparingByMembers<Supports<FoldingRangeClientCapabilities>>();
        }
    }
}
