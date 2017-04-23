using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Capabilities.Server
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ServerCapabilities
    {
// TODO NEXT:
// Create ability for server capabilties to be pulled from registered handlers
// Create the ability to look at the client capabilities to determine what parts we need to answer now (completion for example)

        /// <summary>
        ///  Defines how text documents are synced. Is either a detailed structure defining each notification or
        ///  for backwards compatibility the TextDocumentSyncKind number.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TextDocumentSync TextDocumentSync { get; set; }
        /// <summary>
        ///  The server provides hover support.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool HoverProvider { get; set; }
        /// <summary>
        ///  The server provides completion support.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CompletionOptions CompletionProvider { get; set; }
        /// <summary>
        ///  The server provides signature help support.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SignatureHelpOptions SignatureHelpProvider { get; set; }
        /// <summary>
        ///  The server provides goto definition support.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool DefinitionProvider { get; set; }
        /// <summary>
        ///  The server provides find references support.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool ReferencesProvider { get; set; }
        /// <summary>
        ///  The server provides document highlight support.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool DocumentHighlightProvider { get; set; }
        /// <summary>
        ///  The server provides document symbol support.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool DocumentSymbolProvider { get; set; }
        /// <summary>
        ///  The server provides workspace symbol support.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool WorkspaceSymbolProvider { get; set; }
        /// <summary>
        ///  The server provides code actions.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool CodeActionProvider { get; set; }
        /// <summary>
        ///  The server provides code lens.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CodeLensOptions CodeLensProvider { get; set; }
        /// <summary>
        ///  The server provides document formatting.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool DocumentFormattingProvider { get; set; }
        /// <summary>
        ///  The server provides document range formatting.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool DocumentRangeFormattingProvider { get; set; }
        /// <summary>
        ///  The server provides document formatting on typing.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DocumentOnTypeFormattingOptions DocumentOnTypeFormattingProvider { get; set; }
        /// <summary>
        ///  The server provides rename support.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool RenameProvider { get; set; }
        /// <summary>
        ///  The server provides document link support.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DocumentLinkOptions DocumentLinkProvider { get; set; }
        /// <summary>
        ///  The server provides execute command support.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ExecuteCommandOptions ExecuteCommandProvider { get; set; }
        /// <summary>
        ///  Experimental server capabilities.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, object> Experimental { get; set; } = new Dictionary<string, object>();
    }
}
