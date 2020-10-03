using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    [JsonConverter(typeof(TextDocumentSyncConverter))]
    public class TextDocumentSync
    {
        public TextDocumentSync(TextDocumentSyncKind kind) => Kind = kind;

        public TextDocumentSync(TextDocumentSyncOptions value) => Options = value;
        public bool HasKind => Options == null;
        public TextDocumentSyncKind Kind { get; set; }
        public bool HasOptions => Options != null;
        public TextDocumentSyncOptions? Options { get; set; }
        public object Value => Options ?? (object) Kind;

        public static implicit operator TextDocumentSync(TextDocumentSyncKind value) => new TextDocumentSync(value);

        public static implicit operator TextDocumentSync(TextDocumentSyncOptions value) => new TextDocumentSync(value);
    }
}
