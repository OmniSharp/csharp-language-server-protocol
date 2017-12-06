using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class CompletionParams : TextDocumentPositionParams
    {
        /// <summary>
        /// The completion context. This is only available it the client specifies to send
        /// this using `ClientCapabilities.textDocument.completion.contextSupport === true`
        /// </summary>
        [Optional]
        public CompletionContext Context { get; set; }
    }
}
