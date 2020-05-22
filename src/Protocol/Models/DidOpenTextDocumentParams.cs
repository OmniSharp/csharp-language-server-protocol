using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.DidOpen, Direction.ClientToServer)]
    public class DidOpenTextDocumentParams : IRequest
    {
        /// <summary>
        ///  The document that was opened.
        /// </summary>
        public TextDocumentItem TextDocument { get; set; }
    }
}
