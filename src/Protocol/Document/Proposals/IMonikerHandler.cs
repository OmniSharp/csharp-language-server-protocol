using System;
using System.Collections.Generic;
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
        IRegistration<MonikerRegistrationOptions>,
        ICapability<MonikerCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    public abstract class MonikerHandlerBase : IMonikerHandler
    {
        private readonly MonikerRegistrationOptions _options;

        protected MonikerHandlerBase(MonikerRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public MonikerRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<Moniker>?> Handle(MonikerParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(MonikerCapability capability) => Capability = capability;
        protected MonikerCapability Capability { get; private set; } = null!;
    }

    [Obsolete(Constants.Proposal)]
    public abstract class PartialMonikerHandlerBase :
        AbstractHandlers.PartialResults<MonikerParams, Container<Moniker>, Moniker, MonikerCapability, MonikerRegistrationOptions>, IMonikerHandler
    {
        protected PartialMonikerHandlerBase(MonikerRegistrationOptions registrationOptions, IProgressManager progressManager) : base(
            registrationOptions, progressManager,
            items => new Container<Moniker>(items)
        )
        {
        }

        public virtual Guid Id { get; } = Guid.NewGuid();
    }
}
