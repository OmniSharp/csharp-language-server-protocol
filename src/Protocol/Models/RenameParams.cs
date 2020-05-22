using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.Rename, Direction.ClientToServer)]
    public class RenameParams : ITextDocumentIdentifierParams, IRequest<WorkspaceEdit>, IWorkDoneProgressParams
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

        /// <inheritdoc />
        [Optional]
        public ProgressToken WorkDoneToken { get; set; }
    }
}
