using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Client capabilities specific to the used markdown parser.
    ///
    /// @since 3.16.0
    /// </summary>
    public class MarkdownClientCapabilities
    {
        /// <summary>
        /// The name of the parser.
        /// </summary>
        public string Parser { get; set; } = null!;

        /// <summary>
        /// The version of the parser.
        /// </summary>
        [Optional]
        public string? Version { get; set; }

        /// <summary>
        /// A list of HTML tags that the client allows / supports in
        /// Markdown.
        ///
        /// @since 3.17.0
        /// </summary>
        [Optional]
        public Container<string>? AllowedTags { get; set; }
    }
}
