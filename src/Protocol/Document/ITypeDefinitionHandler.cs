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
    [Parallel, Method(TextDocumentNames.TypeDefinition, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ITypeDefinitionHandler : IJsonRpcRequestHandler<TypeDefinitionParams, LocationOrLocationLinks>, IRegistration<TypeDefinitionRegistrationOptions>, ICapability<TypeDefinitionCapability> { }

    public abstract class TypeDefinitionHandler : ITypeDefinitionHandler
    {
        private readonly TypeDefinitionRegistrationOptions _options;
        public TypeDefinitionHandler(TypeDefinitionRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TypeDefinitionRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<LocationOrLocationLinks> Handle(TypeDefinitionParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(TypeDefinitionCapability capability) => Capability = capability;
        protected TypeDefinitionCapability Capability { get; private set; }
    }
}
