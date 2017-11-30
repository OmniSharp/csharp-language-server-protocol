using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Abstractions;
using OmniSharp.Extensions.LanguageServer.Capabilities.Client;
using OmniSharp.Extensions.LanguageServer.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Serial, Method("textDocument/rangeFormatting")]
    public interface IDocumentRangeFormattingHandler : IRequestHandler<DocumentRangeFormattingParams, TextEditContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DocumentRangeFormattingCapability> { }
}
