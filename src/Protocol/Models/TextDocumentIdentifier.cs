using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class TextDocumentIdentifier
    {
        public TextDocumentIdentifier()
        {

        }

        public TextDocumentIdentifier(DocumentUri uri)
        {
            Uri = uri;
        }

        /// <summary>
        /// The text document's URI.
        /// </summary>
        public DocumentUri Uri { get; set; }
    }
}
