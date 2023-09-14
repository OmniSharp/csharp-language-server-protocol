using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public partial record VersionedTextDocumentIdentifier : TextDocumentIdentifier
    {
        /// <summary>
        /// The version number of this document.
        ///
        /// The version number of a document will increase after each change,
        /// including undo/redo.The number doesn't need to be consecutive.
        /// </summary>
        public int Version { get; init; }

        private string DebuggerDisplay => $"{Uri}@({Version})";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
