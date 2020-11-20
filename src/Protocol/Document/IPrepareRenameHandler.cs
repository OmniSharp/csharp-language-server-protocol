using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel]
    [Method(TextDocumentNames.PrepareRename, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IPrepareRenameHandler : IJsonRpcRequestHandler<PrepareRenameParams, RangeOrPlaceholderRange?>, IRegistration<TextDocumentRegistrationOptions>,
                                             ICapability<RenameCapability>, IDoesNotParticipateInRegistration
    {
    }

    public abstract class PrepareRenameHandler : AbstractHandlers.Request<PrepareRenameParams, RangeOrPlaceholderRange?, TextDocumentRegistrationOptions, RenameCapability> { }
}
