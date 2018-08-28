using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class ServerCapabilities
    {
        // TODO NEXT:
        // Create ability for server capabilties to be pulled from registered handlers
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
        public bool HoverProvider { get; set; }
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
        public bool DefinitionProvider { get; set; }
        /// <summary>
        ///  The server provides find references support.
        /// </summary>
        [Optional]
        public bool ReferencesProvider { get; set; }
        /// <summary>
        ///  The server provides document highlight support.
        /// </summary>
        [Optional]
        public bool DocumentHighlightProvider { get; set; }
        /// <summary>
        ///  The server provides document symbol support.
        /// </summary>
        [Optional]
        public bool DocumentSymbolProvider { get; set; }
        /// <summary>
        ///  The server provides workspace symbol support.
        /// </summary>
        [Optional]
        public bool WorkspaceSymbolProvider { get; set; }
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
        public bool DocumentFormattingProvider { get; set; }
        /// <summary>
        ///  The server provides document range formatting.
        /// </summary>
        [Optional]
        public bool DocumentRangeFormattingProvider { get; set; }
        /// <summary>
        ///  The server provides document formatting on typing.
        /// </summary>
        [Optional]
        public DocumentOnTypeFormattingOptions DocumentOnTypeFormattingProvider { get; set; }
        /// <summary>
        ///  The server provides rename support.
        /// </summary>
        [Optional]
        public bool RenameProvider { get; set; }
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
        public BooleanOr<StaticColorOptions> ColorProvider { get; set; }
        /// <summary>
        /// Workspace specific server capabilities
        /// </summary>
        [Optional]
        public WorkspaceServerCapabilities Workspace { get; set; }
    }

    public class CodeActionOptions
    {
        /// <summary>
        /// CodeActionKinds that this server may return.
        ///
        /// The list of kinds may be generic, such as `CodeActionKind.Refactor`, or the server
        /// may list out every specific kind they provide.
        /// </summary>
        [Optional]
        public Container<CodeActionKind> ProvidedCodeActionKinds { get; set; }
    }
}
