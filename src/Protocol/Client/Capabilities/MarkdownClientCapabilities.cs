using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Client capabilities specific to the used markdown parser.
    ///
    /// @since 3.16.0 - proposed state
    /// </summary>
    public record MarkdownClientCapabilities
    {
        /// <summary>
        /// The name of the parser.
        /// </summary>
        public string Parser { get; set; }

        /// <summary>
        /// The version of the parser.
        /// </summary>
        [Optional]
        public string? Version { get; set; }
    }
}
