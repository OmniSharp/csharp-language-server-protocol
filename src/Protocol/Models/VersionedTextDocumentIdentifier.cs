namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class VersionedTextDocumentIdentifier : TextDocumentIdentifier
    {
        /// <summary>
        /// The version number of this document.
        /// </summary>
        public long Version { get; set; }
    }
}
