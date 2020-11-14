using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Params to show a document.
    ///
    /// @since 3.16.0 - proposed state
    /// </summary>
    [Parallel]
    [Method(WindowNames.ShowDocument, Direction.ServerToClient)]
    public class ShowDocumentParams : IRequest<ShowDocumentResult>
    {
        /// <summary>
        /// The document uri to show.
        /// </summary>
        public DocumentUri Uri { get; set; } = null!;

        /// <summary>
        /// Indicates to show the resource in an external program.
        /// To show for example `https://code.visualstudio.com/`
        /// in the default WEB browser set `external` to `true`.
        /// </summary>
        [Optional]
        public bool? External { get; set; }

        /// <summary>
        /// An optional property to indicate whether the editor
        /// showing the document should take focus or not.
        /// Clients might ignore this property if an external
        /// program is started.
        /// </summary>
        [Optional]
        public bool? TakeFocus { get; set; }

        /// <summary>
        /// An optional selection range if the document is a text
        /// document. Clients might ignore the property if an
        /// external program is started or the file is not a text
        /// file.
        /// </summary>
        [Optional]
        public Range? Selection { get; set; }
    }
}
