using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    using static DocumentNames;
    [Serial, Method(Formatting)]
    public interface IDocumentFormattingHandler : IJsonRpcRequestHandler<DocumentFormattingParams, TextEditContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DocumentFormattingCapability> { }
}
