using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(DocumentNames.DidOpen)]
    public class DidOpenTextDocumentParams : IRequest
    {
        /// <summary>
        ///  The document that was opened.
        /// </summary>
        public TextDocumentItem TextDocument { get; set; }
    }
}
