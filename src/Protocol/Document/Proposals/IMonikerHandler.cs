using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals
{
    [Obsolete(Constants.Proposal)]
    [Parallel]
    [Method(TextDocumentNames.Moniker, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IMonikerHandler :
        IJsonRpcRequestHandler<MonikerParams, Container<Moniker>?>,
        IRegistration<MonikerRegistrationOptions, MonikerCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    public abstract class MonikerHandlerBase : AbstractHandlers.Request<MonikerParams, Container<Moniker>?, MonikerRegistrationOptions, MonikerCapability>, IMonikerHandler
    {
    }

    [Obsolete(Constants.Proposal)]
    public abstract class PartialMonikerHandlerBase :
        AbstractHandlers.PartialResults<MonikerParams, Container<Moniker>?, Moniker, MonikerRegistrationOptions, MonikerCapability>, IMonikerHandler
    {
        protected PartialMonikerHandlerBase(IProgressManager progressManager) : base(progressManager, items => new Container<Moniker>(items))
        {
        }

        public virtual Guid Id { get; } = Guid.NewGuid();
    }
}
