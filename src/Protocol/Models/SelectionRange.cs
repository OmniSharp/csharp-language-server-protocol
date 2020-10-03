using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class SelectionRange
    {
        /// <summary>
        /// The [range](#Range) of this selection range.
        /// </summary>
        public Range Range { get; set; } = null!;

        /// <summary>
        /// The parent selection range containing this range. Therefore `parent.range` must contain `this.range`.
        /// </summary>
        public SelectionRange Parent { get; set; } = null!;

        private string DebuggerDisplay => $"{Range} {{{Parent}}}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
