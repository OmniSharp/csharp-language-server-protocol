using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// Is not a record on purpose...
    /// get; set; for the moment, to allow for replacement of values.
    /// </remarks>
    public interface IServerCapabilities : ICapabilitiesBase
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
        PositionEncodingKind? PositionEncoding { get; set; }

        /// <summary>
        /// Defines how text documents are synced. Is either a detailed structure defining each notification or
        /// for backwards compatibility the TextDocumentSyncKind number.
        /// </summary>
        TextDocumentSync? TextDocumentSync { get; set; }

//        /// <summary>
//        /// Defines how notebook documents are synced.
//        ///
//        /// @since 3.17.0
//        /// </summary>
//        NotebookDocumentSyncOptions.StaticOptions NotebookDocumentSync { get; set; }

        /// <summary>
        /// The server provides hover support.
        /// </summary>
        BooleanOr<HoverRegistrationOptions.StaticOptions>? HoverProvider { get; set; }

        /// <summary>
        /// The server provides completion support.
        /// </summary>
        CompletionRegistrationOptions.StaticOptions? CompletionProvider { get; set; }

        /// <summary>
        /// The server provides signature help support.
        /// </summary>
        SignatureHelpRegistrationOptions.StaticOptions? SignatureHelpProvider { get; set; }

        /// <summary>
        /// The server provides goto definition support.
        /// </summary>
        BooleanOr<DefinitionRegistrationOptions.StaticOptions>? DefinitionProvider { get; set; }

        /// <summary>
        /// The server provides find references support.
        /// </summary>
        BooleanOr<ReferenceRegistrationOptions.StaticOptions>? ReferencesProvider { get; set; }

        /// <summary>
        /// The server provides document highlight support.
        /// </summary>
        BooleanOr<DocumentHighlightRegistrationOptions.StaticOptions>? DocumentHighlightProvider { get; set; }

        /// <summary>
        /// The server provides document symbol support.
        /// </summary>
        BooleanOr<DocumentSymbolRegistrationOptions.StaticOptions>? DocumentSymbolProvider { get; set; }

        /// <summary>
        /// The server provides workspace symbol support.
        /// </summary>
        BooleanOr<WorkspaceSymbolRegistrationOptions.StaticOptions>? WorkspaceSymbolProvider { get; set; }

        /// <summary>
        /// The server provides code actions.
        /// </summary>
        BooleanOr<CodeActionRegistrationOptions.StaticOptions>? CodeActionProvider { get; set; }

        /// <summary>
        /// The server provides code lens.
        /// </summary>
        CodeLensRegistrationOptions.StaticOptions? CodeLensProvider { get; set; }

        /// <summary>
        /// The server provides document formatting.
        /// </summary>
        BooleanOr<DocumentFormattingRegistrationOptions.StaticOptions>? DocumentFormattingProvider { get; set; }

        /// <summary>
        /// The server provides document range formatting.
        /// </summary>
        BooleanOr<DocumentRangeFormattingRegistrationOptions.StaticOptions>? DocumentRangeFormattingProvider { get; set; }

        /// <summary>
        /// The server provides document formatting on typing.
        /// </summary>
        DocumentOnTypeFormattingRegistrationOptions.StaticOptions? DocumentOnTypeFormattingProvider { get; set; }

        /// <summary>
        /// The server provides rename support.
        /// </summary>
        BooleanOr<RenameRegistrationOptions.StaticOptions>? RenameProvider { get; set; }

        /// <summary>
        /// The server provides document link support.
        /// </summary>
        DocumentLinkRegistrationOptions.StaticOptions? DocumentLinkProvider { get; set; }

        /// <summary>
        /// The server provides execute command support.
        /// </summary>
        ExecuteCommandRegistrationOptions.StaticOptions? ExecuteCommandProvider { get; set; }

        /// <summary>
        /// Experimental server capabilities.
        /// </summary>
        IDictionary<string, JToken> Experimental { get; set; }

        /// <summary>
        /// The server provides Goto Type Definition support.
        ///
        /// Since 3.6.0
        /// </summary>
        BooleanOr<TypeDefinitionRegistrationOptions.StaticOptions>? TypeDefinitionProvider { get; set; }

        /// <summary>
        /// The server provides Goto Implementation support.
        ///
        /// Since 3.6.0
        /// </summary>
        BooleanOr<ImplementationRegistrationOptions.StaticOptions>? ImplementationProvider { get; set; }

        /// <summary>
        /// The server provides color provider support.
        ///
        /// Since 3.6.0
        /// </summary>
        BooleanOr<DocumentColorRegistrationOptions.StaticOptions>? ColorProvider { get; set; }

        /// <summary>
        /// The server provides Call Hierarchy support.
        /// </summary>
        BooleanOr<CallHierarchyRegistrationOptions.StaticOptions>? CallHierarchyProvider { get; set; }

        /// <summary>
        /// The server provides Call Hierarchy support.
        /// </summary>
        SemanticTokensRegistrationOptions.StaticOptions? SemanticTokensProvider { get; set; }

        /// <summary>
        /// The server provides Call Hierarchy support.
        /// </summary>
        MonikerRegistrationOptions.StaticOptions? MonikerProvider { get; set; }

        /// <summary>
        /// The server provides folding provider support.
        ///
        /// Since 3.10.0
        /// </summary>
        BooleanOr<FoldingRangeRegistrationOptions.StaticOptions>? FoldingRangeProvider { get; set; }

        /// <summary>
        /// The server provides selection range support.
        ///
        /// Since 3.15.0
        /// </summary>
        BooleanOr<SelectionRangeRegistrationOptions.StaticOptions>? SelectionRangeProvider { get; set; }

        /// <summary>
        /// The server provides on type rename support.
        ///
        /// Since 3.16.0
        /// </summary>
        BooleanOr<LinkedEditingRangeRegistrationOptions.StaticOptions>? LinkedEditingRangeProvider { get; set; }

        /// <summary>
        /// The server provides folding provider support.
        ///
        /// Since 3.14.0
        /// </summary>
        BooleanOr<DeclarationRegistrationOptions.StaticOptions>? DeclarationProvider { get; set; }

        /// <summary>
        /// The server provides type hierarchy support.
        ///
        /// @since 3.17.0
        /// </summary>
        BooleanOr<TypeHierarchyRegistrationOptions.StaticOptions>? TypeHierarchyProvider { get; set; }

        /// <summary>
        /// The server provides inline values.
        ///
        /// @since 3.17.0
        /// </summary>
        BooleanOr<InlineValueRegistrationOptions.StaticOptions>? InlineValueProvider { get; set; }

        /// <summary>
        /// The server provides inlay hints.
        ///
        /// @since 3.17.0
        /// </summary>
        BooleanOr<InlayHintRegistrationOptions.StaticOptions>? InlayHintProvider { get; set; }

        /// <summary>
        /// The server has support for pull model diagnostics.
        ///
        /// @since 3.17.0
        /// </summary>
        DiagnosticsRegistrationOptions.StaticOptions? DiagnosticProvider { get; set; }
    }
}
