using System;
using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    // TODO: Rename to confirm with spec VersionedTextDocumentIdentifier
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record RequiredVersionedTextDocumentIdentifier : TextDocumentIdentifier
    {
        /// <summary>
        /// The version number of this document.
        /// </summary>
        public int Version { get; init; }

        private string DebuggerDisplay => $"{Uri}@({Version})";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
