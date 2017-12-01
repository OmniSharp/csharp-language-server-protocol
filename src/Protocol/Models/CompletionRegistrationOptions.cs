using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
    }
}
