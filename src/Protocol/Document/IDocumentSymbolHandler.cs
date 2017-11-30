using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Abstractions;
using OmniSharp.Extensions.LanguageServer.Capabilities.Client;
using OmniSharp.Extensions.LanguageServer.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Parallel, Method("textDocument/documentSymbol")]
    public interface IDocumentSymbolHandler : IRequestHandler<DocumentSymbolParams, SymbolInformationContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DocumentSymbolCapability> { }
}
