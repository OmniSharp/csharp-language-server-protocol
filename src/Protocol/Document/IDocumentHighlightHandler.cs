using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static DocumentNames;
    public static partial class DocumentNames
    {
        public const string DocumentHighlight = "textDocument/documentHighlight";
    }

    [Parallel, Method(DocumentHighlight)]
    public interface IDocumentHighlightHandler : IRequestHandler<TextDocumentPositionParams, DocumentHighlightContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DocumentHighlightCapability> { }
}
