using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class TextDocumentClientCapabilities
    {
        public Supports<TextDocumentSyncClientCapabilities> Synchronization { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/completion`
        /// </summary>
        public Supports<CompletionClientCapabilities> Completion { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/hover`
        /// </summary>
        public Supports<HoverClientCapabilities> Hover { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/signatureHelp`
        /// </summary>
        public Supports<SignatureHelpClientCapabilities> SignatureHelp { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/references`
        /// </summary>
        public Supports<ReferenceClientCapabilities> References { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentHighlight`
        /// </summary>
        public Supports<DocumentHighlightClientCapabilities> DocumentHighlight { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentSymbol`
        /// </summary>
        public Supports<DocumentSymbolClientCapabilities> DocumentSymbol { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/formatting`
        /// </summary>
        public Supports<DocumentFormattingClientCapabilities> Formatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/rangeFormatting`
        /// </summary>
        public Supports<DocumentRangeFormattingClientCapabilities> RangeFormatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/onTypeFormatting`
        /// </summary>
        public Supports<DocumentOnTypeFormattingClientCapabilities> OnTypeFormatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/definition`
        /// </summary>
        public Supports<DefinitionClientCapabilities> Definition { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/declaration`
        /// </summary>
        public Supports<DeclarationClientCapabilities> Declaration { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/codeAction`
        /// </summary>
        public Supports<CodeActionClientCapabilities> CodeAction { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/codeLens`
        /// </summary>
        public Supports<CodeLensClientCapabilities> CodeLens { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentLink`
        /// </summary>
        public Supports<DocumentLinkClientCapabilities> DocumentLink { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/rename`
        /// </summary>
        public Supports<RenameClientCapabilities> Rename { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/typeDefinition`
        ///
        /// Since 3.6.0
        /// </summary>
        public Supports<TypeDefinitionClientCapabilities> TypeDefinition { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/implementation`
        ///
        /// Since 3.6.0
        /// </summary>
        public Supports<ImplementationClientCapabilities> Implementation { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentColor` and the `textDocument/colorPresentation` request.
        ///
        /// Since 3.6.0
        /// </summary>
        public Supports<ColorProviderClientCapabilities> ColorProvider { get; set; }

        /// <summary>
        /// The server provides folding provider support.
        ///
        /// Since 3.10.0
        /// </summary>
        public Supports<FoldingRangeClientCapabilities> FoldingRange { get; set; }

        /// <summary>
        /// The server provides selection provider support.
        ///
        /// Since 3.15.0
        /// </summary>
        public Supports<SelectionRangeClientCapabilities> SelectionRange { get; set; }

        /// <summary>
        /// Capabilities specific to `textDocument/publishDiagnostics`.
        /// </summary>
        public Supports<PublishDiagnosticsClientCapabilities> PublishDiagnostics { get; set; }
    }
}
