using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// A special text edit to provide an insert and a replace operation.
    ///
    /// @since 3.16.0 - proposed state
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class InsertReplaceEdit
    {
        /// <summary>
        /// The string to be inserted.
        /// </summary>
        public string NewText { get; set; }

        /// <summary>
        /// The range if the insert is requested
        /// </summary>
        public Range Insert { get; set; }

        /// <summary>
        /// The range if the replace is requested.
        /// </summary>
        public Range Replace { get; set; }

        private string DebuggerDisplay => $"{Insert} / {Replace} {( string.IsNullOrWhiteSpace(NewText) ? string.Empty : NewText.Length > 30 ? NewText.Substring(0, 30) : NewText )}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
