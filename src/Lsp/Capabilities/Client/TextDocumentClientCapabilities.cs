using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Capabilities.Client
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TextDocumentClientCapabilities
    {
        public Capability<SynchronizationCapability> Synchronization { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/completion`
        /// </summary>
        public Capability<CompletionCapability> Completion { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/hover`
        /// </summary>
        public Capability<HoverCapability> Hover { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/signatureHelp`
        /// </summary>
        public Capability<SignatureHelpCapability> SignatureHelp { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/references`
        /// </summary>
        public Capability<ReferencesCapability> References { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentHighlight`
        /// </summary>
        public Capability<DocumentHighlightCapability> DocumentHighlight { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentSymbol`
        /// </summary>
        public Capability<DocumentSymbolCapability> DocumentSymbol { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/formatting`
        /// </summary>
        public Capability<FormattingCapability> Formatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/rangeFormatting`
        /// </summary>
        public Capability<RangeFormattingCapability> RangeFormatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/onTypeFormatting`
        /// </summary>
        public Capability<OnTypeFormattingCapability> OnTypeFormatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/definition`
        /// </summary>
        public Capability<DefinitionCapability> Definition { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/codeAction`
        /// </summary>
        public Capability<CodeActionCapability> CodeAction { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/codeLens`
        /// </summary>
        public Capability<CodeLensCapability> CodeLens { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentLink`
        /// </summary>
        public Capability<DocumentLinkCapability> DocumentLink { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/rename`
        /// </summary>
        public Capability<RenameCapability> Rename { get; set; }
    }
}
