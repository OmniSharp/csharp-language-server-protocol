using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DidSaveTextDocumentParams : ITextDocumentIdentifierParams
    {
        /// <summary>
        ///  The document that was saved.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        ///  Optional the content when saved. Depends on the includeText value
        ///  when the save notifcation was requested.
        /// </summary>
        [Optional]
        public string Text { get; set; }
    }
}
