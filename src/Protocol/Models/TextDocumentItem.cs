﻿namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
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
        public long Version { get; set; }

        /// <summary>
        /// The content of the opened text document.
        /// </summary>
        public string Text { get; set; }
    }
}
