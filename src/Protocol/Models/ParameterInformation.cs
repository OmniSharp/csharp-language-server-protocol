using System.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Represents a parameter of a callable-signature. A parameter can
    /// have a label and a doc-comment.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ParameterInformation
    {
        /// <summary>
        /// The label of this parameter. Will be shown in
        /// the UI.
        /// </summary>
        public ParameterInformationLabel Label { get; set; } = null!;

        /// <summary>
        /// The human-readable doc-comment of this parameter. Will be shown
        /// in the UI but can be omitted.
        /// </summary>
        [Optional]
        public StringOrMarkupContent? Documentation { get; set; }

        private string DebuggerDisplay => $"{Label}{( Documentation != null ? $" {Documentation}" : string.Empty )}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
