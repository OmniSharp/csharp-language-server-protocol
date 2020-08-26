using System.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Represents the signature of something callable. A signature
    /// can have a label, like a function-name, a doc-comment, and
    /// a set of parameters.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class SignatureInformation
    {
        /// <summary>
        /// The label of this signature. Will be shown in
        /// the UI.
        /// </summary>
        public string Label { get; set; } = null!;

        /// <summary>
        /// The human-readable doc-comment of this signature. Will be shown
        /// in the UI but can be omitted.
        /// </summary>
        [Optional]
        public StringOrMarkupContent? Documentation { get; set; }

        /// <summary>
        /// The parameters of this signature.
        /// </summary>
        [Optional]
        public Container<ParameterInformation>? Parameters { get; set; }

        /// <summary>
        /// The index of the active parameter.
        ///
        /// If provided, this is used in place of `SignatureHelp.activeParameter`.
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        [Optional]
        public int? ActiveParameter { get; set; }

        private string DebuggerDisplay => $"{Label}{Documentation?.ToString() ?? ""}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
