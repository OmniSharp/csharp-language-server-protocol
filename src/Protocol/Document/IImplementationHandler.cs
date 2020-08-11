using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel]
    [Method(TextDocumentNames.Implementation, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IImplementationHandler : IJsonRpcRequestHandler<ImplementationParams, LocationOrLocationLinks>,
                                              IRegistration<ImplementationRegistrationOptions>, ICapability<ImplementationCapability>
    {
    }

    public abstract class ImplementationHandler : AbstractHandlers.Request<ImplementationParams, LocationOrLocationLinks, ImplementationCapability,
                                                      ImplementationRegistrationOptions>, IImplementationHandler
    {
        protected ImplementationHandler(ImplementationRegistrationOptions registrationOptions) : base(
            registrationOptions
        )
        {
        }
    }
}
