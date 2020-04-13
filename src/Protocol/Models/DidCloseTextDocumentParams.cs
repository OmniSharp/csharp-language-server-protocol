using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(DocumentNames.DidClose)]
    public class DidCloseTextDocumentParams : ITextDocumentIdentifierParams, IRequest
    {
        /// <summary>
        ///  The document that was closed.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }
    }
}
