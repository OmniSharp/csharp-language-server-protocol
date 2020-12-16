namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public interface IGeneralClientCapabilities
    {
        /// <summary>
        /// Client capabilities specific to regular expressions.
        ///
        /// @since 3.16.0
        /// </summary>
        RegularExpressionsClientCapabilities? RegularExpressions { get; set; }

        /// <summary>
        /// Client capabilities specific to the client's markdown parser.
        ///
        /// @since 3.16.0
        /// </summary>
        MarkdownClientCapabilities? Markdown { get; set; }
    }
}