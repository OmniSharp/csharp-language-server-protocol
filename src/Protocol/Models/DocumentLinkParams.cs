using OmniSharp.Extensions.Embedded.MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentLinkParams : ITextDocumentIdentifierParams , IRequest<DocumentLinkContainer>
    {
        /// <summary>
        /// The document to provide document links for.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }
    }
}
