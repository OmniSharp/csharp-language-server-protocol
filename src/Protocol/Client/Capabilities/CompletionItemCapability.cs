using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class CompletionItemCapability
    {
        /// <summary>
        /// Client supports snippets as insert text.
        ///
        /// A snippet can define tab stops and placeholders with `$1`, `$2`
        /// and `${3:foo}`. `$0` defines the final tab stop, it defaults to
        /// the end of the snippet. Placeholders with equal identifiers are linked,
        /// that is typing in one will update others too.
        /// </summary>
        [Optional]
        public bool SnippetSupport { get; set; }

        /// <summary>
        /// Client supports commit characters on a completion item.
        /// </summary>
        [Optional]
        public bool CommitCharactersSupport { get; set; }

        /// <summary>
        /// Client supports the follow content formats for the documentation
        /// property. The order describes the preferred format of the client.
        /// </summary>
        [Optional]
        public Container<MarkupKind> DocumentationFormat { get; set; }
    }
}
