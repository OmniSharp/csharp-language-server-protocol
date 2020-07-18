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
    [Parallel, Method(TextDocumentNames.DocumentColor, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IDocumentColorHandler : IJsonRpcRequestHandler<DocumentColorParams, Container<ColorPresentation>>, IRegistration<DocumentColorRegistrationOptions>, ICapability<ColorProviderCapability> { }

    public abstract class DocumentColorHandler : IDocumentColorHandler
    {
        private readonly DocumentColorRegistrationOptions _options;
        public DocumentColorHandler(DocumentColorRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentColorRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<ColorPresentation>> Handle(DocumentColorParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(ColorProviderCapability capability) => Capability = capability;
        protected ColorProviderCapability Capability { get; private set; }
    }
}
