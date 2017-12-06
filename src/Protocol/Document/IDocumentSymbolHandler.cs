using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static DocumentNames;
    public static partial class DocumentNames
    {
        public const string DocumentSymbol = "textDocument/documentSymbol";
    }

    [Parallel, Method(DocumentSymbol)]
    public interface IDocumentSymbolHandler : IRequestHandler<DocumentSymbolParams, DocumentSymbolInformationContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DocumentSymbolCapability> { }
}
