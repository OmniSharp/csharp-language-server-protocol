using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentSymbolParams : ITextDocumentIdentifierParams
    {
        /// <summary>
        /// The text document.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }
    }
}
