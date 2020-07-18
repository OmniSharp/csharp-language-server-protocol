using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel, Method(TextDocumentNames.SignatureHelp, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ISignatureHelpHandler : IJsonRpcRequestHandler<SignatureHelpParams, SignatureHelp>, IRegistration<SignatureHelpRegistrationOptions>, ICapability<SignatureHelpCapability> { }

    public abstract class SignatureHelpHandler : ISignatureHelpHandler
    {
        private readonly SignatureHelpRegistrationOptions _options;
        public SignatureHelpHandler(SignatureHelpRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public SignatureHelpRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<SignatureHelp> Handle(SignatureHelpParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(SignatureHelpCapability capability) => Capability = capability;
        protected SignatureHelpCapability Capability { get; private set; }
    }
}
