using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/codeAction")]
    public interface ICodeActionHandler : IRequestHandler<CodeActionParams, CommandContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<CodeActionCapability> { }
}
