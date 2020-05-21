using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class ServerCapabilities
    {
        // TODO NEXT:
        // Create ability for server capabilities to be pulled from registered handlers
        // Create the ability to look at the client capabilities to determine what parts we need to answer now (completion for example)

        /// <summary>
        ///  Defines how text documents are synced. Is either a detailed structure defining each notification or
        ///  for backwards compatibility the TextDocumentSyncKind number.
        /// </summary>
        [Optional]
        public TextDocumentSync TextDocumentSync { get; set; }

        /// <summary>
        ///  The server provides hover support.
        /// </summary>
        [Optional]
        public BooleanOr<HoverOptions> HoverProvider { get; set; }

        /// <summary>
        ///  The server provides completion support.
        /// </summary>
        [Optional]
        public CompletionOptions CompletionProvider { get; set; }

        /// <summary>
        ///  The server provides signature help support.
        /// </summary>
        [Optional]
        public SignatureHelpOptions SignatureHelpProvider { get; set; }

        /// <summary>
        ///  The server provides goto definition support.
        /// </summary>
        [Optional]
        public BooleanOr<DefinitionOptions> DefinitionProvider { get; set; }

        /// <summary>
        ///  The server provides find references support.
        /// </summary>
        [Optional]
        public BooleanOr<ReferencesOptions> ReferencesProvider { get; set; }

        /// <summary>
        ///  The server provides document highlight support.
        /// </summary>
        [Optional]
        public BooleanOr<DocumentHighlightOptions> DocumentHighlightProvider { get; set; }

        /// <summary>
        ///  The server provides document symbol support.
        /// </summary>
        [Optional]
        public BooleanOr<DocumentSymbolOptions> DocumentSymbolProvider { get; set; }

        /// <summary>
        ///  The server provides workspace symbol support.
        /// </summary>
        [Optional]
        public BooleanOr<WorkspaceSymbolOptions> WorkspaceSymbolProvider { get; set; }

        /// <summary>
        ///  The server provides code actions.
        /// </summary>
        [Optional]
        public BooleanOr<CodeActionOptions> CodeActionProvider { get; set; }

        /// <summary>
        ///  The server provides code lens.
        /// </summary>
        [Optional]
        public CodeLensOptions CodeLensProvider { get; set; }

        /// <summary>
        ///  The server provides document formatting.
        /// </summary>
        [Optional]
        public BooleanOr<DocumentFormattingOptions> DocumentFormattingProvider { get; set; }

        /// <summary>
        ///  The server provides document range formatting.
        /// </summary>
        [Optional]
        public BooleanOr<DocumentRangeFormattingOptions> DocumentRangeFormattingProvider { get; set; }

        /// <summary>
        ///  The server provides document formatting on typing.
        /// </summary>
        [Optional]
        public DocumentOnTypeFormattingOptions DocumentOnTypeFormattingProvider { get; set; }

        /// <summary>
        ///  The server provides rename support.
        /// </summary>
        [Optional]
        public BooleanOr<RenameOptions> RenameProvider { get; set; }

        /// <summary>
        ///  The server provides document link support.
        /// </summary>
        [Optional]
        public DocumentLinkOptions DocumentLinkProvider { get; set; }

        /// <summary>
        ///  The server provides execute command support.
        /// </summary>
        [Optional]
        public ExecuteCommandOptions ExecuteCommandProvider { get; set; }

        /// <summary>
        ///  Experimental server capabilities.
        /// </summary>
        [Optional]
        public IDictionary<string, JToken> Experimental { get; set; } = new Dictionary<string, JToken>();

        /// <summary>
        /// The server provides Goto Type Definition support.
        ///
        /// Since 3.6.0
        /// </summary>
        [Optional]
        public BooleanOr<TypeDefinitionOptions> TypeDefinitionProvider { get; set; }

        /// <summary>
        /// The server provides Goto Implementation support.
        ///
        /// Since 3.6.0
        /// </summary>
        [Optional]
        public BooleanOr<ImplementationOptions> ImplementationProvider { get; set; }

        /// <summary>
        /// The server provides color provider support.
        ///
        /// Since 3.6.0
        /// </summary>
        [Optional]
        public BooleanOr<DocumentColorOptions> ColorProvider { get; set; }

        /// <summary>
        /// The server provides Call Hierarchy support.
        /// </summary>
        [Optional]
        [Obsolete(Constants.Proposal)]
        public BooleanOr<CallHierarchyOptions> CallHierarchyProvider { get; set; }

        /// <summary>
        /// The server provides Call Hierarchy support.
        /// </summary>
        [Optional]
        [Obsolete(Constants.Proposal)]
        public SemanticTokensOptions SemanticTokensProvider { get; set; }

        /// <summary>
        /// The server provides folding provider support.
        ///
        /// Since 3.10.0
        /// </summary>
        public BooleanOr<FoldingRangeOptions> FoldingRangeProvider { get; set; }

        /// <summary>
        /// The server provides selection range support.
        ///
        /// Since 3.15.0
        /// </summary>
        public BooleanOr<SelectionRangeOptions> SelectionRangeProvider { get; set; }

        /// <summary>
        /// The server provides folding provider support.
        ///
        /// Since 3.14.0
        /// </summary>
        public BooleanOr<DeclarationOptions> DeclarationProvider { get; set; }

        /// <summary>
        /// Workspace specific server capabilities
        /// </summary>
        [Optional]
        public WorkspaceServerCapabilities Workspace { get; set; }
    }
}
