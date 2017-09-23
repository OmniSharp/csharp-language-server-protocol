using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Abstractions;
using OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Client;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

namespace OmniSharp.Extensions.LanguageServerProtocol.Protocol.Document
{
    [Method("textDocument/rangeFormatting")]
    public interface IDocumentRangeFormattingHandler : IRequestHandler<DocumentRangeFormattingParams, TextEditContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DocumentRangeFormattingCapability> { }
}