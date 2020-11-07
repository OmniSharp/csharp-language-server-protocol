using System;
using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record VersionedTextDocumentIdentifier(DocumentUri Uri, int Version = 0) : TextDocumentIdentifier(Uri)
    {
        public VersionedTextDocumentIdentifier() : this(null!, 0)
        {
        }

        private string DebuggerDisplay => $"{Uri}@({Version})";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
