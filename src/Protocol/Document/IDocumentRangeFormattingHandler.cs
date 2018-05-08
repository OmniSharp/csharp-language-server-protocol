using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static DocumentNames;
    public static partial class DocumentNames
    {
        public const string RangeFormatting = "textDocument/rangeFormatting";
    }

    [Serial, Method(RangeFormatting)]
    public interface IDocumentRangeFormattingHandler : IJsonRpcRequestHandler<DocumentRangeFormattingParams, TextEditContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DocumentRangeFormattingCapability> { }
}
