using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class CompletionItemCapabilityOptions
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
        public Container<MarkupKind>? DocumentationFormat { get; set; }

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
        public Supports<CompletionItemTagSupportCapabilityOptions?> TagSupport { get; set; }

        /// <summary>
        /// Client support insert replace edit to control different behavior if a
        /// completion item is inserted in the text or should replace text.
        ///
        /// @since 3.16.0 - Proposed state
        /// </summary>
        [Optional]
        public bool InsertReplaceSupport { get; set; }

        /// <summary>
        /// Client supports to resolve `additionalTextEdits` in the `completionItem/resolve`
        /// request. So servers can postpone computing them.
        ///
        /// @since 3.16.0 - Proposed state
        /// </summary>
        [Optional]
        public bool ResolveAdditionalTextEditsSupport { get; set; }

        /// <summary>
        /// Indicates which properties a client can resolve lazily on a completion
        /// item. Before version 3.16.0 only the predefined properties `documentation`
        /// and `details` could be resolved lazily.
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        [Optional]
        public CompletionItemCapabilityResolveSupportOptions? ResolveSupport { get; set; }
    }
}
