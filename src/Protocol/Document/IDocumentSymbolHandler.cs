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
    [Parallel, Method(TextDocumentNames.DocumentSymbol, Direction.ClientToServer)]
    public interface IDocumentSymbolHandler : IJsonRpcRequestHandler<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer>, IRegistration<DocumentSymbolRegistrationOptions>, ICapability<DocumentSymbolCapability> { }

    public abstract class DocumentSymbolHandler : IDocumentSymbolHandler
    {
        private readonly DocumentSymbolRegistrationOptions _options;
        public DocumentSymbolHandler(DocumentSymbolRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentSymbolRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<SymbolInformationOrDocumentSymbolContainer> Handle(DocumentSymbolParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentSymbolCapability capability) => Capability = capability;
        protected DocumentSymbolCapability Capability { get; private set; }
    }

    public static class DocumentSymbolExtensions
    {
        public static IDisposable OnDocumentSymbol(
            this ILanguageServerRegistry registry,
            Func<DocumentSymbolParams, DocumentSymbolCapability, CancellationToken, Task<SymbolInformationOrDocumentSymbolContainer>>
                handler,
            DocumentSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentSymbolRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentSymbol,
                new LanguageProtocolDelegatingHandlers.Request<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer, DocumentSymbolCapability,
                    DocumentSymbolRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentSymbol(
            this ILanguageServerRegistry registry,
            Func<DocumentSymbolParams, CancellationToken, Task<SymbolInformationOrDocumentSymbolContainer>> handler,
            DocumentSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentSymbolRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentSymbol,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer,
                    DocumentSymbolRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentSymbol(
            this ILanguageServerRegistry registry,
            Func<DocumentSymbolParams, Task<SymbolInformationOrDocumentSymbolContainer>> handler,
            DocumentSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentSymbolRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentSymbol,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer,
                    DocumentSymbolRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentSymbol(
            this ILanguageServerRegistry registry,
            Action<DocumentSymbolParams, IObserver<IEnumerable<SymbolInformationOrDocumentSymbol>>, DocumentSymbolCapability,
                CancellationToken> handler,
            DocumentSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentSymbolRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentSymbol,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer,
                        SymbolInformationOrDocumentSymbol, DocumentSymbolCapability, DocumentSymbolRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(), x => new SymbolInformationOrDocumentSymbolContainer(x)));
        }

        public static IDisposable OnDocumentSymbol(
            this ILanguageServerRegistry registry,
            Action<DocumentSymbolParams, IObserver<IEnumerable<SymbolInformationOrDocumentSymbol>>, DocumentSymbolCapability>
                handler,
            DocumentSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentSymbolRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentSymbol,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer,
                        SymbolInformationOrDocumentSymbol, DocumentSymbolCapability, DocumentSymbolRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(), x => new SymbolInformationOrDocumentSymbolContainer(x)));
        }

        public static IDisposable OnDocumentSymbol(
            this ILanguageServerRegistry registry,
            Action<DocumentSymbolParams, IObserver<IEnumerable<SymbolInformationOrDocumentSymbol>>, CancellationToken> handler,
            DocumentSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentSymbolRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentSymbol,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer,
                        SymbolInformationOrDocumentSymbol, DocumentSymbolRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(), x => new SymbolInformationOrDocumentSymbolContainer(x)));
        }

        public static IDisposable OnDocumentSymbol(
            this ILanguageServerRegistry registry,
            Action<DocumentSymbolParams, IObserver<IEnumerable<SymbolInformationOrDocumentSymbol>>> handler,
            DocumentSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentSymbolRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentSymbol,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer,
                        SymbolInformationOrDocumentSymbol, DocumentSymbolRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(), x => new SymbolInformationOrDocumentSymbolContainer(x)));
        }

        public static IRequestProgressObservable<IEnumerable<SymbolInformationOrDocumentSymbol>, SymbolInformationOrDocumentSymbolContainer> RequestDocumentSymbol(
            this ITextDocumentLanguageClient mediator,
            DocumentSymbolParams @params,
            CancellationToken cancellationToken = default)
        {
            return mediator.ProgressManager.MonitorUntil(@params, x => new SymbolInformationOrDocumentSymbolContainer(x), cancellationToken);
        }
    }
}
