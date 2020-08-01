﻿using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class TextDocumentItem : TextDocumentIdentifier
    {
        /// <summary>
        /// The text document's language identifier.
        /// </summary>
        public string LanguageId { get; set; }

        /// <summary>
        /// The version number of this document (it will strictly increase after each
        /// change, including undo/redo).
        /// </summary>
        public int? Version { get; set; }

        /// <summary>
        /// The content of the opened text document.
        /// </summary>
        public string Text { get; set; }

        private string DebuggerDisplay => $"({LanguageId}@{Version}) {Uri}";
        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
