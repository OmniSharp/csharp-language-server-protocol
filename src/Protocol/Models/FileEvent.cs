namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    ///  An event describing a file change.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
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

        private string DebuggerDisplay => $"[{Type}] {Uri}";
        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
