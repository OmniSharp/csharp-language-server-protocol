using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// General client capabilities.
    /// </summary>
    public class GeneralClientCapabilities : CapabilitiesBase, IGeneralClientCapabilities
    {
        /// <summary>
        /// Client capabilities specific to regular expressions.
        ///
        /// @since 3.16.0
        /// </summary>
        [Optional]
        public RegularExpressionsClientCapabilities? RegularExpressions { get; set; }

        /// <summary>
        /// Client capabilities specific to the client's markdown parser.
        ///
        /// @since 3.16.0
        /// </summary>
        [Optional] public MarkdownClientCapabilities? Markdown { get; set; }
    }
}