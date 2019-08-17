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

        public TextDocumentIdentifier(Uri uri)
        {
            Uri = uri;
        }

        /// <summary>
        /// The text document's URI.
        /// </summary>
        [JsonConverter(typeof(AbsoluteUriConverter))]
        public Uri Uri { get; set; }
    }
}
