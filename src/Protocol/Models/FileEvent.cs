namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    ///  An event describing a file change.
    /// </summary>
    public class FileEvent
    {
        /// <summary>
        ///  The file's URI.
        /// </summary>
        public DocumentUri Uri { get; set; }

        /// <summary>
        ///  The change type.
        /// </summary>
        public FileChangeType Type { get; set; }
    }
}
