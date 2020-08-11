using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Rename file Options
    /// </summary>
    public class RenameFileOptions
    {
        /// <summary>
        /// Overwrite target if existing. Overwrite wins over `ignoreIfExists`
        /// </summary>
        [Optional]
        public bool Overwrite { get; set; }

        /// <summary>
        /// Ignores if target exists.
        /// </summary>
        [Optional]
        public bool IgnoreIfExists { get; set; }
    }
}
