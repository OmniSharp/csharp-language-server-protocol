using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public TextDocumentSync TextDocumentSync { get; set; }

        /// <summary>
        ///  The server provides hover support.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public BooleanOr<HoverOptions> HoverProvider { get; set; }

        /// <summary>
        ///  The server provides completion support.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public CompletionOptions CompletionProvider { get; set; }

        /// <summary>
        ///  The server provides signature help support.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public SignatureHelpOptions SignatureHelpProvider { get; set; }

        /// <summary>
        ///  The server provides goto definition support.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public BooleanOr<DefinitionOptions> DefinitionProvider { get; set; }

        /// <summary>
        ///  The server provides find references support.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public BooleanOr<ReferencesOptions> ReferencesProvider { get; set; }

        /// <summary>
        ///  The server provides document highlight support.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public BooleanOr<DocumentHighlightOptions> DocumentHighlightProvider { get; set; }

        /// <summary>
        ///  The server provides document symbol support.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public BooleanOr<DocumentSymbolOptions> DocumentSymbolProvider { get; set; }

        /// <summary>
        ///  The server provides workspace symbol support.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public BooleanOr<WorkspaceSymbolOptions> WorkspaceSymbolProvider { get; set; }

        /// <summary>
        ///  The server provides code actions.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public BooleanOr<CodeActionOptions> CodeActionProvider { get; set; }

        /// <summary>
        ///  The server provides code lens.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public CodeLensOptions CodeLensProvider { get; set; }

        /// <summary>
        ///  The server provides document formatting.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public BooleanOr<DocumentFormattingOptions> DocumentFormattingProvider { get; set; }

        /// <summary>
        ///  The server provides document range formatting.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public BooleanOr<DocumentRangeFormattingOptions> DocumentRangeFormattingProvider { get; set; }

        /// <summary>
        ///  The server provides document formatting on typing.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public DocumentOnTypeFormattingOptions DocumentOnTypeFormattingProvider { get; set; }

        /// <summary>
        ///  The server provides rename support.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public BooleanOr<RenameOptions> RenameProvider { get; set; }

        /// <summary>
        ///  The server provides document link support.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public DocumentLinkOptions DocumentLinkProvider { get; set; }

        /// <summary>
        ///  The server provides execute command support.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public ExecuteCommandOptions ExecuteCommandProvider { get; set; }

        /// <summary>
        ///  Experimental server capabilities.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public IDictionary<string, JsonElement> Experimental { get; set; } = new Dictionary<string, JsonElement>();

        /// <summary>
        /// The server provides Goto Type Definition support.
        ///
        /// Since 3.6.0
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public BooleanOr<TypeDefinitionOptions> TypeDefinitionProvider { get; set; }

        /// <summary>
        /// The server provides Goto Implementation support.
        ///
        /// Since 3.6.0
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public BooleanOr<ImplementationOptions> ImplementationProvider { get; set; }

        /// <summary>
        /// The server provides color provider support.
        ///
        /// Since 3.6.0
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public BooleanOr<DocumentColorOptions> ColorProvider { get; set; }

        /// <summary>
        /// The server provides Call Hierarchy support.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        [Obsolete(Constants.Proposal)]
        public BooleanOr<CallHierarchyOptions> CallHierarchyProvider { get; set; }

        /// <summary>
        /// The server provides Call Hierarchy support.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public WorkspaceServerCapabilities Workspace { get; set; }
    }
}
