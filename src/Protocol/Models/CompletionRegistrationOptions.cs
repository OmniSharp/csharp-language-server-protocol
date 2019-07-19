using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class CompletionRegistrationOptions : TextDocumentRegistrationOptions, ICompletionOptions
    {
        /// <summary>
        /// The characters that trigger completion automatically.
        /// </summary>
        [Optional]
        public Container<string> TriggerCharacters { get; set; }

        /// <summary>
        /// The server provides support to resolve additional
        /// information for a completion item.
        /// </summary>
        [Optional]
        public bool ResolveProvider { get; set; }

        /// <summary>
        /// The list of all possible characters that commit a completion. This field can be used
        /// if clients don't support individual commit characters per completion item. See
        /// `ClientCapabilities.textDocument.completion.completionItem.commitCharactersSupport`
        ///
        /// Since 3.2.0
        /// </summary>
        [Optional]
        public Container<string> AllCommitCharacters { get; set; }
    }
}
