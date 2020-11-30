using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// An event describing a file change.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record FileEvent
    {
        /// <summary>
        /// The file's URI.
        /// </summary>
        public DocumentUri Uri { get; init; }

        /// <summary>
        /// The change type.
        /// </summary>
        public FileChangeType Type { get; init; }

        private string DebuggerDisplay => $"[{Type}] {Uri}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
