using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(DocumentNames.DidChange)]
    public class DidChangeTextDocumentParams : IRequest
    {
        /// <summary>
        ///  The document that did change. The version number points
        ///  to the version after all provided content changes have
        ///  been applied.
        /// </summary>
        public VersionedTextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        ///  The actual content changes.
        /// </summary>
        public Container<TextDocumentContentChangeEvent> ContentChanges { get; set; }
    }
}
