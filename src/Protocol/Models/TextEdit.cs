using System.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [GenerateContainer]
    public record TextEdit
    {
        /// <summary>
        /// The range of the text document to be manipulated. To insert
        /// text into a document create a range where start === end.
        /// </summary>
        public Range Range { get; init; }

        /// <summary>
        /// The string to be inserted. For delete operations use an
        /// empty string.
        /// </summary>
        public string NewText { get; init; }

        private string DebuggerDisplay => $"{Range} {( string.IsNullOrWhiteSpace(NewText) ? string.Empty : NewText.Length > 30 ? NewText.Substring(0, 30) : NewText )}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
