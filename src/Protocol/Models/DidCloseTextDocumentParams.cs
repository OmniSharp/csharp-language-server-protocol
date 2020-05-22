using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.DidClose, Direction.ClientToServer)]
    public class DidCloseTextDocumentParams : ITextDocumentIdentifierParams, IRequest
    {
        /// <summary>
        ///  The document that was closed.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }
    }
}
