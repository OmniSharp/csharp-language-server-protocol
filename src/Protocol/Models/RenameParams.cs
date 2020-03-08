using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class RenameParams : ITextDocumentIdentifierParams, IRequest<WorkspaceEdit>
    {
        /// <summary>
        /// The document to format.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The position at which this request was sent.
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// The new name of the symbol. If the given name is not valid the
        /// request must return a [ResponseError](#ResponseError) with an
        /// appropriate message set.
        /// </summary>
        public string NewName { get; set; }
    }
}
