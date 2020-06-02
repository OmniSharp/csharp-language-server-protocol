using FluentAssertions.Equivalency;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace Lsp.Tests
{
    public static class FluentAssertionsExtensions
    {
        public static EquivalencyAssertionOptions<T> ConfigureForSupports<T>(this EquivalencyAssertionOptions<T> options, ILogger logger = null)
        {
            return options
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
                .ComparingByMembers<Supports<TagSupportCapability>>()
                .ComparingByMembers<Supports<CompletionItemTagSupportCapability>>()
                ;
        }
    }
}
