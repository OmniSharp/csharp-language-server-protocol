using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record OptionalVersionedTextDocumentIdentifier : TextDocumentIdentifier
    {
        /// <summary>
        /// The version number of this document. If an optional versioned text document
        /// identifier is sent from the server to the client and the file is not
        /// open in the editor(the server has not received an open notification
        /// before) the server can send `null` to indicate that the version is
        /// known and the content on disk is the master(as specified with document
        /// content ownership).
        ///
        /// The version number of a document will increase after each change,
        /// including undo/redo.The number doesn't need to be consecutive.
        /// </summary>
        public int? Version { get; init; }

        private string DebuggerDisplay => $"{Uri}@({Version})";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
