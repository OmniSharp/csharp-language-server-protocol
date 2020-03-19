using MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class SignatureHelpParams : WorkDoneTextDocumentPositionParams, IRequest<SignatureHelp>
    {
        /// <summary>
        /// The signature help context. This is only available if the client specifies
        /// to send this using the client capability  `textDocument.signatureHelp.contextSupport === true`
        ///
        /// @since 3.15.0
        /// </summary>
        public SignatureHelpContext Context { get; set; }
    }
}
