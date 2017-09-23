using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Abstractions;
using OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Client;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/formatting")]
    public interface IDocumentFormattingHandler : IRequestHandler<DocumentFormattingParams, TextEditContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DocumentFormattingCapability> { }
}