namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class TextDocumentSync
    {
        public TextDocumentSync(TextDocumentSyncKind kind)
        {
            Kind = kind;
        }
        public TextDocumentSync(TextDocumentSyncOptions value)
        {
            Options = value;
        }
        public bool HasKind => this.Options == null;
        public TextDocumentSyncKind Kind { get; set; }
        public bool HasOptions => this.Options != null;
        public TextDocumentSyncOptions Options { get; set; }
        public object Value => Options ?? (object)Kind;

        public static implicit operator TextDocumentSync(TextDocumentSyncKind value)
        {
            return new TextDocumentSync(value);
        }

        public static implicit operator TextDocumentSync(TextDocumentSyncOptions value)
        {
            return new TextDocumentSync(value);
        }
    }
}
