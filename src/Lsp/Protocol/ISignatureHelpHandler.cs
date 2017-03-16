using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(SignatureHelpRegistrationOptions))]
    [Method("textDocument/signatureHelp")]
    public interface ISignatureHelpHandler : IRequestHandler<TextDocumentPositionParams, SignatureHelp> { }
}