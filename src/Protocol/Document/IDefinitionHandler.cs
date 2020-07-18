using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel, Method(TextDocumentNames.Definition, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IDefinitionHandler : IJsonRpcRequestHandler<DefinitionParams, LocationOrLocationLinks>, IRegistration<DefinitionRegistrationOptions>,
        ICapability<DefinitionCapability>
    {
    }

    public abstract class DefinitionHandler : IDefinitionHandler
    {
        private readonly DefinitionRegistrationOptions _options;

        public DefinitionHandler(DefinitionRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DefinitionRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<LocationOrLocationLinks> Handle(DefinitionParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DefinitionCapability capability) => Capability = capability;
        protected DefinitionCapability Capability { get; private set; }
    }
}
