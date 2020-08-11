using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    /// <summary>
    /// Defines how the host (editor) should sync document changes to the language server.
    /// </summary>
    [JsonConverter(typeof(NumberEnumConverter))]
    public enum TextDocumentSyncKind
    {
        /// <summary>
        /// Documents should not be synced at all.
        /// </summary>
        None = 0,

        /// <summary>
        /// Documents are synced by always sending the full content
        /// of the document.
        /// </summary>
        Full = 1,

        /// <summary>
        /// Documents are synced by sending the full content on open.
        /// After that only incremental updates to the document are
        /// send.
        /// </summary>
        Incremental = 2,
    }
}
