using System.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Represents a folding range.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class FoldingRange
    {
        /// <summary>
        /// The zero-based line number from where the folded range starts.
        /// </summary>
        public long StartLine { get; set; }

        /// <summary>
        /// The zero-based character offset from where the folded range starts. If not defined, defaults to the length of the start line.
        /// </summary>
        [Optional]
        public long? StartCharacter { get; set; }

        /// <summary>
        /// The zero-based line number where the folded range ends.
        /// </summary>
        public long EndLine { get; set; }

        /// <summary>
        /// The zero-based character offset before the folded range ends. If not defined, defaults to the length of the end line.
        /// </summary>
        [Optional]
        public long? EndCharacter { get; set; }

        /// <summary>
        /// Describes the kind of the folding range such as `comment' or 'region'. The kind
        /// is used to categorize folding ranges and used by commands like 'Fold all comments'. See
        /// [FoldingRangeKind](#FoldingRangeKind) for an enumeration of standardized kinds.
        /// </summary>
        [Optional]
        public FoldingRangeKind? Kind { get; set; }

        private string DebuggerDisplay =>
            $"[start: (line: {StartLine}{( StartCharacter.HasValue ? $", char: {StartCharacter}" : string.Empty )}), end: (line: {EndLine}, char: {( EndCharacter.HasValue ? $", char: {EndCharacter}" : string.Empty )})]";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
