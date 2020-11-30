using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    /// <summary>
    /// Save options.
    /// </summary>
    public record SaveOptions
    {
        /// <summary>
        /// The client is supposed to include the content on save.
        /// </summary>
        [Optional]
        public bool IncludeText { get; init; }
    }
}
