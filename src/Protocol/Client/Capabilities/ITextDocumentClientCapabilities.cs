namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public interface ITextDocumentClientCapabilities : ICapabilitiesBase
    {
        Supports<SynchronizationCapability?> Synchronization { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/completion`
        /// </summary>
        Supports<CompletionCapability?> Completion { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/hover`
        /// </summary>
        Supports<HoverCapability?> Hover { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/signatureHelp`
        /// </summary>
        Supports<SignatureHelpCapability?> SignatureHelp { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/references`
        /// </summary>
        Supports<ReferenceCapability?> References { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentHighlight`
        /// </summary>
        Supports<DocumentHighlightCapability?> DocumentHighlight { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentSymbol`
        /// </summary>
        Supports<DocumentSymbolCapability?> DocumentSymbol { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/formatting`
        /// </summary>
        Supports<DocumentFormattingCapability?> Formatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/rangeFormatting`
        /// </summary>
        Supports<DocumentRangeFormattingCapability?> RangeFormatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/onTypeFormatting`
        /// </summary>
        Supports<DocumentOnTypeFormattingCapability?> OnTypeFormatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/definition`
        /// </summary>
        Supports<DefinitionCapability?> Definition { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/declaration`
        /// </summary>
        Supports<DeclarationCapability?> Declaration { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/codeAction`
        /// </summary>
        Supports<CodeActionCapability?> CodeAction { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/codeLens`
        /// </summary>
        Supports<CodeLensCapability?> CodeLens { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentLink`
        /// </summary>
        Supports<DocumentLinkCapability?> DocumentLink { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/rename`
        /// </summary>
        Supports<RenameCapability?> Rename { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/typeDefinition`
        ///
        /// Since 3.6.0
        /// </summary>
        Supports<TypeDefinitionCapability?> TypeDefinition { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/implementation`
        ///
        /// Since 3.6.0
        /// </summary>
        Supports<ImplementationCapability?> Implementation { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentColor` and the `textDocument/colorPresentation` request.
        ///
        /// Since 3.6.0
        /// </summary>
        Supports<ColorProviderCapability?> ColorProvider { get; set; }

        /// <summary>
        /// The server provides folding provider support.
        ///
        /// Since 3.10.0
        /// </summary>
        Supports<FoldingRangeCapability?> FoldingRange { get; set; }

        /// <summary>
        /// The server provides selection provider support.
        ///
        /// Since 3.15.0
        /// </summary>
        Supports<SelectionRangeCapability?> SelectionRange { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/linkedEditingRange` request.
        ///
        /// Since 3.16.0
        /// </summary>
        Supports<LinkedEditingRangeClientCapabilities?> LinkedEditingRange { get; set; }

        /// <summary>
        /// Capabilities specific to `textDocument/publishDiagnostics`.
        /// </summary>
        Supports<PublishDiagnosticsCapability?> PublishDiagnostics { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/callHierarchy`.
        ///
        /// @since 3.16.0
        /// </summary>
        Supports<CallHierarchyCapability?> CallHierarchy { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/semanticTokens`
        ///
        /// @since 3.16.0
        /// </summary>
        Supports<SemanticTokensCapability?> SemanticTokens { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/moniker`
        ///
        /// @since 3.16.0
        /// </summary>
        Supports<MonikerCapability?> Moniker { get; set; }
    }
}