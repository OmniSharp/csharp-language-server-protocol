using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    using static DocumentNames;
    [Parallel, Method(FoldingRange)]
    public interface IFoldingRangeHandler : IJsonRpcRequestHandler<FoldingRangeRequestParam, Container<FoldingRange>>, IRegistration<TextDocumentRegistrationOptions>, ICapability<FoldingRangeCapability> { }
}
