using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// Is not a record on purpose...
    /// get; set; for the moment, to allow for replacement of values.
    /// </remarks>
    public class ServerCapabilities : CapabilitiesBase, IServerCapabilities
    {
        /// <summary>
        /// The position encoding the server picked from the encodings offered
        /// by the client via the client capability `general.positionEncodings`.
        ///
        /// If the client didn't provide any position encodings the only valid
        /// value that a server can return is 'utf-16'.
        ///
        /// If omitted it defaults to 'utf-16'.
        ///
        /// @since 3.17.0
        /// </summary>
        public PositionEncodingKind? PositionEncoding { get; set; }

        /// <summary>
        /// Defines how text documents are synced. Is either a detailed structure defining each notification or
        /// for backwards compatibility the TextDocumentSyncKind number.
        /// </summary>
        [Optional]
        public TextDocumentSync? TextDocumentSync { get; set; }

        /// <summary>
        /// Defines how notebook documents are synced.
        ///
        /// @since 3.17.0
        /// </summary>
        [Optional]
        public NotebookDocumentSyncOptions? NotebookDocumentSync { get; set; }

        /// <summary>
        /// The server provides hover support.
        /// </summary>
        [Optional]
        public BooleanOr<HoverRegistrationOptions.StaticOptions>? HoverProvider { get; set; }

        /// <summary>
        /// The server provides completion support.
        /// </summary>
        [Optional]
        public CompletionRegistrationOptions.StaticOptions? CompletionProvider { get; set; }

        /// <summary>
        /// The server provides signature help support.
        /// </summary>
        [Optional]
        public SignatureHelpRegistrationOptions.StaticOptions? SignatureHelpProvider { get; set; }

        /// <summary>
        /// The server provides goto definition support.
        /// </summary>
        [Optional]
        public BooleanOr<DefinitionRegistrationOptions.StaticOptions>? DefinitionProvider { get; set; }

        /// <summary>
        /// The server provides find references support.
        /// </summary>
        [Optional]
        public BooleanOr<ReferenceRegistrationOptions.StaticOptions>? ReferencesProvider { get; set; }

        /// <summary>
        /// The server provides document highlight support.
        /// </summary>
        [Optional]
        public BooleanOr<DocumentHighlightRegistrationOptions.StaticOptions>? DocumentHighlightProvider { get; set; }

        /// <summary>
        /// The server provides document symbol support.
        /// </summary>
        [Optional]
        public BooleanOr<DocumentSymbolRegistrationOptions.StaticOptions>? DocumentSymbolProvider { get; set; }

        /// <summary>
        /// The server provides workspace symbol support.
        /// </summary>
        [Optional]
        public BooleanOr<WorkspaceSymbolRegistrationOptions.StaticOptions>? WorkspaceSymbolProvider { get; set; }

        /// <summary>
        /// The server provides code actions.
        /// </summary>
        [Optional]
        public BooleanOr<CodeActionRegistrationOptions.StaticOptions>? CodeActionProvider { get; set; }

        /// <summary>
        /// The server provides code lens.
        /// </summary>
        [Optional]
        public CodeLensRegistrationOptions.StaticOptions? CodeLensProvider { get; set; }

        /// <summary>
        /// The server provides document formatting.
        /// </summary>
        [Optional]
        public BooleanOr<DocumentFormattingRegistrationOptions.StaticOptions>? DocumentFormattingProvider { get; set; }

        /// <summary>
        /// The server provides document range formatting.
        /// </summary>
        [Optional]
        public BooleanOr<DocumentRangeFormattingRegistrationOptions.StaticOptions>? DocumentRangeFormattingProvider { get; set; }

        /// <summary>
        /// The server provides document formatting on typing.
        /// </summary>
        [Optional]
        public DocumentOnTypeFormattingRegistrationOptions.StaticOptions? DocumentOnTypeFormattingProvider { get; set; }

        /// <summary>
        /// The server provides rename support.
        /// </summary>
        [Optional]
        public BooleanOr<RenameRegistrationOptions.StaticOptions>? RenameProvider { get; set; }

        /// <summary>
        /// The server provides document link support.
        /// </summary>
        [Optional]
        public DocumentLinkRegistrationOptions.StaticOptions? DocumentLinkProvider { get; set; }

        /// <summary>
        /// The server provides execute command support.
        /// </summary>
        [Optional]
        public ExecuteCommandRegistrationOptions.StaticOptions? ExecuteCommandProvider { get; set; }

        /// <summary>
        /// Experimental server capabilities.
        /// </summary>
        [Optional]
        public IDictionary<string, JToken> Experimental { get; set; } = new Dictionary<string, JToken>();

        /// <summary>
        /// The server provides Goto Type Definition support.
        ///
        /// Since 3.6.0
        /// </summary>
        [Optional]
        public BooleanOr<TypeDefinitionRegistrationOptions.StaticOptions>? TypeDefinitionProvider { get; set; }

        /// <summary>
        /// The server provides Goto Implementation support.
        ///
        /// Since 3.6.0
        /// </summary>
        [Optional]
        public BooleanOr<ImplementationRegistrationOptions.StaticOptions>? ImplementationProvider { get; set; }

        /// <summary>
        /// The server provides color provider support.
        ///
        /// Since 3.6.0
        /// </summary>
        [Optional]
        public BooleanOr<DocumentColorRegistrationOptions.StaticOptions>? ColorProvider { get; set; }

        /// <summary>
        /// The server provides Call Hierarchy support.
        /// </summary>
        [Optional]
        public BooleanOr<CallHierarchyRegistrationOptions.StaticOptions>? CallHierarchyProvider { get; set; }

        /// <summary>
        /// The server provides Call Hierarchy support.
        /// </summary>
        [Optional]
        public SemanticTokensRegistrationOptions.StaticOptions? SemanticTokensProvider { get; set; }

        /// <summary>
        /// The server provides Call Hierarchy support.
        /// </summary>
        [Optional]
        public MonikerRegistrationOptions.StaticOptions? MonikerProvider { get; set; }

        /// <summary>
        /// The server provides folding provider support.
        ///
        /// Since 3.10.0
        /// </summary>
        [Optional]
        public BooleanOr<FoldingRangeRegistrationOptions.StaticOptions>? FoldingRangeProvider { get; set; }

        /// <summary>
        /// The server provides selection range support.
        ///
        /// Since 3.15.0
        /// </summary>
        [Optional]
        public BooleanOr<SelectionRangeRegistrationOptions.StaticOptions>? SelectionRangeProvider { get; set; }

        /// <summary>
        /// The server provides on type rename support.
        ///
        /// Since 3.16.0
        /// </summary>
        [Optional]
        public BooleanOr<LinkedEditingRangeRegistrationOptions.StaticOptions>? LinkedEditingRangeProvider { get; set; }

        /// <summary>
        /// The server provides folding provider support.
        ///
        /// Since 3.14.0
        /// </summary>
        [Optional]
        public BooleanOr<DeclarationRegistrationOptions.StaticOptions>? DeclarationProvider { get; set; }

        /// <summary>
        /// The server provides type hierarchy support.
        ///
        /// @since 3.17.0
        /// </summary>
        [Optional]
        public BooleanOr<TypeHierarchyRegistrationOptions.StaticOptions>? TypeHierarchyProvider { get; set; }

        /// <summary>
        /// The server provides inline values.
        ///
        /// @since 3.17.0
        /// </summary>
        [Optional]
        public BooleanOr<InlineValueRegistrationOptions.StaticOptions>? InlineValueProvider { get; set; }

        /// <summary>
        /// The server provides inlay hints.
        ///
        /// @since 3.17.0
        /// </summary>
        [Optional]
        public BooleanOr<InlayHintRegistrationOptions.StaticOptions>? InlayHintProvider { get; set; }

        /// <summary>
        /// The server has support for pull model diagnostics.
        ///
        /// @since 3.17.0
        /// </summary>
        [Optional]
        public DiagnosticsRegistrationOptions.StaticOptions? DiagnosticProvider { get; set; }

        /// <summary>
        /// Workspace specific server capabilities
        /// </summary>
        [Optional]
        public WorkspaceServerCapabilities? Workspace { get; set; }
    }
}
