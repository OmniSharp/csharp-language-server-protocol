using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentLinkParams : ITextDocumentIdentifierParams
    {
        /// <summary>
        /// The document to provide document links for.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }
    }
}
