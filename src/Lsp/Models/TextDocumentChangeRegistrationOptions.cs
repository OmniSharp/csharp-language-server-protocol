using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Capabilities.Server;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    /// <summary>
    ///  Descibe options to be used when registered for text document change events.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TextDocumentChangeRegistrationOptions : TextDocumentRegistrationOptions
    {
        /// <summary>
        ///  How documents are synced to the server. See TextDocumentSyncKind.Full
        ///  and TextDocumentSyncKindIncremental.
        /// </summary>
        public TextDocumentSyncKind SyncKind { get; set; }
    }
}