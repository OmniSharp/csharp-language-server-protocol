using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/signatureHelp")]
    public interface ISignatureHelpHandler : IRequestHandler<TextDocumentPositionParams, SignatureHelp>, IRegistration<SignatureHelpRegistrationOptions> { }
}