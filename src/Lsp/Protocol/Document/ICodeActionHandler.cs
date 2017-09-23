using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Abstractions;
using OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Client;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/codeAction")]
    public interface ICodeActionHandler : IRequestHandler<CodeActionParams, CommandContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<CodeActionCapability> { }
}
