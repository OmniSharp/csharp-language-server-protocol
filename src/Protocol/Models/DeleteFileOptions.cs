using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Delete file Options
    /// </summary>
    public class DeleteFileOptions
    {
        /// <summary>
        /// Delete the content recursively if a folder is denoted.
        /// </summary>
        [Optional]
        public bool Recursive { get; set; }

        /// <summary>
        /// Ignore the operation if the file doesn't exist.
        /// </summary>
        [Optional]
        public bool IgnoreIfNotExists { get; set; }
    }
}
