using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel, Method(TextDocumentNames.DocumentHighlight, Direction.ClientToServer)]
    public interface IDocumentHighlightHandler : IJsonRpcRequestHandler<DocumentHighlightParams, DocumentHighlightContainer>, IRegistration<DocumentHighlightRegistrationOptions>, ICapability<DocumentHighlightCapability> { }

    public abstract class DocumentHighlightHandler : IDocumentHighlightHandler
    {
        private readonly DocumentHighlightRegistrationOptions _options;
        public DocumentHighlightHandler(DocumentHighlightRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentHighlightRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<DocumentHighlightContainer> Handle(DocumentHighlightParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentHighlightCapability capability) => Capability = capability;
        protected DocumentHighlightCapability Capability { get; private set; }
    }

    public static class DocumentHighlightExtensions
    {
public static ILanguageServerRegistry OnDocumentHighlight(this ILanguageServerRegistry registry,
            Func<DocumentHighlightParams, DocumentHighlightCapability, CancellationToken, Task<DocumentHighlightContainer>>
                handler,
            DocumentHighlightRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentHighlight,
                new LanguageProtocolDelegatingHandlers.Request<DocumentHighlightParams, DocumentHighlightContainer, DocumentHighlightCapability,
                    DocumentHighlightRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnDocumentHighlight(this ILanguageServerRegistry registry,
            Func<DocumentHighlightParams, CancellationToken, Task<DocumentHighlightContainer>> handler,
            DocumentHighlightRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentHighlight,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentHighlightParams, DocumentHighlightContainer,
                    DocumentHighlightRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnDocumentHighlight(this ILanguageServerRegistry registry,
            Func<DocumentHighlightParams, Task<DocumentHighlightContainer>> handler,
            DocumentHighlightRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentHighlight,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentHighlightParams, DocumentHighlightContainer,
                    DocumentHighlightRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnDocumentHighlight(this ILanguageServerRegistry registry,
            Action<DocumentHighlightParams, IObserver<IEnumerable<DocumentHighlight>>, DocumentHighlightCapability,
                CancellationToken> handler,
            DocumentHighlightRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentHighlight,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentHighlightParams, DocumentHighlightContainer,
                        DocumentHighlight, DocumentHighlightCapability, DocumentHighlightRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(),
                        x => new DocumentHighlightContainer(x)));
        }

public static ILanguageServerRegistry OnDocumentHighlight(this ILanguageServerRegistry registry,
            Action<DocumentHighlightParams, IObserver<IEnumerable<DocumentHighlight>>, DocumentHighlightCapability>
                handler,
            DocumentHighlightRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentHighlight,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentHighlightParams, DocumentHighlightContainer,
                        DocumentHighlight, DocumentHighlightCapability, DocumentHighlightRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(),
                        x => new DocumentHighlightContainer(x)));
        }

public static ILanguageServerRegistry OnDocumentHighlight(this ILanguageServerRegistry registry,
            Action<DocumentHighlightParams, IObserver<IEnumerable<DocumentHighlight>>, CancellationToken> handler,
            DocumentHighlightRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentHighlight,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentHighlightParams, DocumentHighlightContainer,
                        DocumentHighlight, DocumentHighlightRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(),
                        x => new DocumentHighlightContainer(x)));
        }

public static ILanguageServerRegistry OnDocumentHighlight(this ILanguageServerRegistry registry,
            Action<DocumentHighlightParams, IObserver<IEnumerable<DocumentHighlight>>> handler,
            DocumentHighlightRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentHighlight,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentHighlightParams, DocumentHighlightContainer,
                        DocumentHighlight, DocumentHighlightRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(),
                        x => new DocumentHighlightContainer(x)));
        }

        public static IRequestProgressObservable<IEnumerable<DocumentHighlight>, DocumentHighlightContainer> RequestDocumentHighlight(
            this ITextDocumentLanguageClient mediator,
            DocumentHighlightParams @params,
            CancellationToken cancellationToken = default)
        {
            return mediator.ProgressManager.MonitorUntil(@params, x => new DocumentHighlightContainer(x), cancellationToken);
        }
    }
}
