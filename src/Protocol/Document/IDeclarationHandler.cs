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
    [Method(TextDocumentNames.Declaration, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IDeclarationHandler : IJsonRpcRequestHandler<DeclarationParams, LocationOrLocationLinks>, IRegistration<DeclarationRegistrationOptions, DeclarationCapability>
    {
    }

    public abstract class DeclarationHandler : AbstractHandlers.Base<DeclarationRegistrationOptions, DeclarationCapability>, IDeclarationHandler
    {
        public abstract Task<LocationOrLocationLinks> Handle(DeclarationParams request, CancellationToken cancellationToken);
    }
}
