using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    ///  The parameters send in a will save text document notification.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class WillSaveTextDocumentParams
    {
        /// <summary>
        ///  The document that will be saved.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        ///  The 'TextDocumentSaveReason'.
        /// </summary>
        public TextDocumentSaveReason Reason { get; set; }
    }
}