using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

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

        /// <summary>
		/// Client supports the deprecated property on a completion item.
		/// </summary>
        [Optional]
        public bool DeprecatedSupport { get; set; }

        /// <summary>
		/// Client supports the preselect property on a completion item.
		/// </summary>
        [Optional]
        public bool PreselectSupport { get; set; }

        /// <summary>
        /// Client supports the tag property on a completion item. Clients supporting
        /// tags have to handle unknown tags gracefully. Clients especially need to
        /// preserve unknown tags when sending a completion item back to the server in
        /// a resolve call.
        ///
        /// @since 3.15.0
        /// </summary>
        [Optional]
        public Supports<CompletionItemTagSupportCapability> TagSupport { get; set; }
    }
}
