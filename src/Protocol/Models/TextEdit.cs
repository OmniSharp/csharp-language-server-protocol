using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class TextEdit
    {
        /// <summary>
        /// The range of the text document to be manipulated. To insert
        /// text into a document create a range where start === end.
        /// </summary>
        public Range Range { get; init; } = null!;

        /// <summary>
        /// The string to be inserted. For delete operations use an
        /// empty string.
        /// </summary>
        public string NewText { get; init; } = string.Empty;

        private string DebuggerDisplay => $"{Range} {( string.IsNullOrWhiteSpace(NewText) ? string.Empty : NewText.Length > 30 ? NewText.Substring(0, 30) : NewText )}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
