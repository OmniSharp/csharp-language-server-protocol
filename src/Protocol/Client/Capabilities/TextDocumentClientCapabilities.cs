using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class TextDocumentClientCapabilities
    {
        public Supports<SynchronizationCapability> Synchronization { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/completion`
        /// </summary>
        public Supports<CompletionCapability> Completion { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/hover`
        /// </summary>
        public Supports<HoverCapability> Hover { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/signatureHelp`
        /// </summary>
        public Supports<SignatureHelpCapability> SignatureHelp { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/references`
        /// </summary>
        public Supports<ReferenceCapability> References { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentHighlight`
        /// </summary>
        public Supports<DocumentHighlightCapability> DocumentHighlight { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentSymbol`
        /// </summary>
        public Supports<DocumentSymbolCapability> DocumentSymbol { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/formatting`
        /// </summary>
        public Supports<DocumentFormattingCapability> Formatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/rangeFormatting`
        /// </summary>
        public Supports<DocumentRangeFormattingCapability> RangeFormatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/onTypeFormatting`
        /// </summary>
        public Supports<DocumentOnTypeFormattingCapability> OnTypeFormatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/definition`
        /// </summary>
        public Supports<DefinitionCapability> Definition { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/declaration`
        /// </summary>
        public Supports<DeclarationCapability> Declaration { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/codeAction`
        /// </summary>
        public Supports<CodeActionCapability> CodeAction { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/codeLens`
        /// </summary>
        public Supports<CodeLensCapability> CodeLens { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentLink`
        /// </summary>
        public Supports<DocumentLinkCapability> DocumentLink { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/rename`
        /// </summary>
        public Supports<RenameCapability> Rename { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/typeDefinition`
        ///
        /// Since 3.6.0
        /// </summary>
        public Supports<TypeDefinitionCapability> TypeDefinition { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/implementation`
        ///
        /// Since 3.6.0
        /// </summary>
        public Supports<ImplementationCapability> Implementation { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentColor` and the `textDocument/colorPresentation` request.
        ///
        /// Since 3.6.0
        /// </summary>
        public Supports<ColorProviderCapability> ColorProvider { get; set; }

        /// <summary>
        /// The server provides folding provider support.
        ///
        /// Since 3.10.0
        /// </summary>
        public Supports<FoldingRangeCapability> FoldingRange { get; set; }

        /// <summary>
        /// The server provides selection provider support.
        ///
        /// Since 3.15.0
        /// </summary>
        public Supports<SelectionRangeCapability> SelectionRange { get; set; }

        /// <summary>
        /// Capabilities specific to `textDocument/publishDiagnostics`.
        /// </summary>
        public Supports<PublishDiagnosticsCapability> PublishDiagnostics { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/callHierarchy`.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        public Supports<CallHierarchyCapability> CallHierarchy { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/semanticTokens`
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        public Supports<SemanticTokensCapability> SemanticTokens { get; set; }
    }
}
