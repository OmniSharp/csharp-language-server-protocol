using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Client capabilities specific to regular expressions.
    ///
    /// @since 3.16.0 - proposed state
    /// </summary>
    public class RegularExpressionsClientCapabilities
    {
        /// <summary>
        /// The engine's name.
        /// </summary>
        public string Engine { get; set; } = null!;

        /// <summary>
        /// The engine's version.
        /// </summary>
        [Optional]
        public string? Version { get; set; }
    }
}
