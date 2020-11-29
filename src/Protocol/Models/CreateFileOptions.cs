using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Options to create a file.
    /// </summary>
    public record CreateFileOptions
    {
        /// <summary>
        /// Overwrite existing file. Overwrite wins over `ignoreIfExists`
        /// </summary>
        [Optional]
        public bool Overwrite { get; init; }

        /// <summary>
        /// Ignore if exists.
        /// </summary>
        [Optional]
        public bool IgnoreIfExists { get; init; }
    }
}
