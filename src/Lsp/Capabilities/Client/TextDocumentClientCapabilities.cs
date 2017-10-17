using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Capabilities.Client
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TextDocumentClientCapabilities
    {
        public Supports<SynchronizationCapability> Synchronization { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeSynchronization() => Synchronization.IsSupported;

        /// <summary>
        /// Capabilities specific to the `textDocument/completion`
        /// </summary>
        public Supports<CompletionCapability> Completion { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeCompletion() => Completion.IsSupported;

        /// <summary>
        /// Capabilities specific to the `textDocument/hover`
        /// </summary>
        public Supports<HoverCapability> Hover { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeHover() => Hover.IsSupported;

        /// <summary>
        /// Capabilities specific to the `textDocument/signatureHelp`
        /// </summary>
        public Supports<SignatureHelpCapability> SignatureHelp { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeSignatureHelp() => SignatureHelp.IsSupported;

        /// <summary>
        /// Capabilities specific to the `textDocument/references`
        /// </summary>
        public Supports<ReferencesCapability> References { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeReferences() => References.IsSupported;

        /// <summary>
        /// Capabilities specific to the `textDocument/documentHighlight`
        /// </summary>
        public Supports<DocumentHighlightCapability> DocumentHighlight { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeDocumentHighlight() => DocumentHighlight.IsSupported;

        /// <summary>
        /// Capabilities specific to the `textDocument/documentSymbol`
        /// </summary>
        public Supports<DocumentSymbolCapability> DocumentSymbol { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeDocumentSymbol() => DocumentSymbol.IsSupported;

        /// <summary>
        /// Capabilities specific to the `textDocument/formatting`
        /// </summary>
        public Supports<DocumentFormattingCapability> Formatting { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeFormatting() => Formatting.IsSupported;

        /// <summary>
        /// Capabilities specific to the `textDocument/rangeFormatting`
        /// </summary>
        public Supports<DocumentRangeFormattingCapability> RangeFormatting { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeRangeFormatting() => RangeFormatting.IsSupported;

        /// <summary>
        /// Capabilities specific to the `textDocument/onTypeFormatting`
        /// </summary>
        public Supports<DocumentOnTypeFormattingCapability> OnTypeFormatting { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeOnTypeFormatting() => OnTypeFormatting.IsSupported;

        /// <summary>
        /// Capabilities specific to the `textDocument/definition`
        /// </summary>
        public Supports<DefinitionCapability> Definition { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeDefinition() => Definition.IsSupported;

        /// <summary>
        /// Capabilities specific to the `textDocument/codeAction`
        /// </summary>
        public Supports<CodeActionCapability> CodeAction { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeCodeAction() => CodeAction.IsSupported;

        /// <summary>
        /// Capabilities specific to the `textDocument/codeLens`
        /// </summary>
        public Supports<CodeLensCapability> CodeLens { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeCodeLens() => CodeLens.IsSupported;

        /// <summary>
        /// Capabilities specific to the `textDocument/documentLink`
        /// </summary>
        public Supports<DocumentLinkCapability> DocumentLink { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeDocumentLink() => DocumentLink.IsSupported;

        /// <summary>
        /// Capabilities specific to the `textDocument/rename`
        /// </summary>
        public Supports<RenameCapability> Rename { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeRename() => Rename.IsSupported;
    }
}
