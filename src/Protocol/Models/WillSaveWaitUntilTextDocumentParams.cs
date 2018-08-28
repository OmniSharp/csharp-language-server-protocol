using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    ///  The parameters send in a will save text document notification.
    /// </summary>
    public class WillSaveWaitUntilTextDocumentParams : IRequest<TextEditContainer>
    {
        /// <summary>
        ///  The document that will be saved.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        ///  The 'TextDocumentSaveReason'.
        /// </summary>
        public TextDocumentSaveReason Reason { get; set; }
    }
}
