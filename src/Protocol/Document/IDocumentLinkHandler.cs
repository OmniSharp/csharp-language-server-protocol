using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
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
    [Parallel, Method(TextDocumentNames.DocumentLink, Direction.ClientToServer)]
    public interface IDocumentLinkHandler<TData> : IJsonRpcRequestHandler<DocumentLinkParams<TData>, DocumentLinkContainer<TData>>,
        IRegistration<DocumentLinkRegistrationOptions>, ICapability<DocumentLinkCapability> where TData : CanBeResolvedData
    {
    }

    [Parallel, Method(TextDocumentNames.DocumentLinkResolve, Direction.ClientToServer)]
    public interface IDocumentLinkResolveHandler<TData> : ICanBeResolvedHandler<DocumentLink<TData>, TData> where TData : CanBeResolvedData
    {
    }

    public abstract class DocumentLinkHandler<TData> : IDocumentLinkHandler<TData>, IDocumentLinkResolveHandler<TData> where TData : CanBeResolvedData
    {
        private readonly DocumentLinkRegistrationOptions _options;
        private readonly Guid _id = Guid.NewGuid();

        public DocumentLinkHandler(DocumentLinkRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentLinkRegistrationOptions GetRegistrationOptions() => _options;

        public abstract Task<DocumentLinkContainer<TData>> Handle(DocumentLinkParams<TData> request, CancellationToken cancellationToken);

        public abstract Task<DocumentLink<TData>> Handle(DocumentLink<TData> request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentLinkCapability capability) => Capability = capability;
        protected DocumentLinkCapability Capability { get; private set; }
        Guid ICanBeIdentifiedHandler.Id => _id;
    }

    public static class DocumentLinkExtensions
    {
        public static ILanguageServerRegistry OnDocumentLink<TData>(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams<TData>, DocumentLinkCapability, CancellationToken, Task<DocumentLinkContainer<TData>>> handler,
            DocumentLinkRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                new LanguageProtocolDelegatingHandlers.Request<DocumentLinkParams<TData>, DocumentLinkContainer<TData>, DocumentLinkCapability,
                    DocumentLinkRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static ILanguageServerRegistry OnDocumentLink<TData>(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams<TData>, DocumentLinkCapability, CancellationToken, Task<DocumentLinkContainer<TData>>> handler,
            Func<DocumentLink<TData>, DocumentLinkCapability, CancellationToken, Task<DocumentLink<TData>>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link, cap, token) => Task.FromException<DocumentLink<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                        new LanguageProtocolDelegatingHandlers.Request<DocumentLinkParams<TData>, DocumentLinkContainer<TData>, DocumentLinkCapability,
                            DocumentLinkRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions))
                    .AddHandler(TextDocumentNames.DocumentLinkResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink<TData>, DocumentLinkCapability, DocumentLinkRegistrationOptions, TData>(
                            id,
                            resolveHandler, registrationOptions
                        ));
        }

        public static ILanguageServerRegistry OnDocumentLink<TData>(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams<TData>, CancellationToken, Task<DocumentLinkContainer<TData>>> handler,
            DocumentLinkRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentLinkParams<TData>, DocumentLinkContainer<TData>,
                    DocumentLinkRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static ILanguageServerRegistry OnDocumentLink<TData>(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams<TData>, CancellationToken, Task<DocumentLinkContainer<TData>>> handler,
            Func<DocumentLink<TData>, CancellationToken, Task<DocumentLink<TData>>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link, token) => Task.FromException<DocumentLink<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                    new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentLinkParams<TData>, DocumentLinkContainer<TData>,
                        DocumentLinkRegistrationOptions>(
                        id,
                        handler,
                        registrationOptions)).AddHandler(TextDocumentNames.DocumentLinkResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink<TData>, DocumentLinkRegistrationOptions, TData>(
                        id,
                        resolveHandler,
                        registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink<TData>(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams<TData>, Task<DocumentLinkContainer<TData>>> handler,
            DocumentLinkRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentLinkParams<TData>, DocumentLinkContainer<TData>,
                    DocumentLinkRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static ILanguageServerRegistry OnDocumentLink<TData>(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams<TData>, Task<DocumentLinkContainer<TData>>> handler,
            Func<DocumentLink<TData>, Task<DocumentLink<TData>>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link) => Task.FromException<DocumentLink<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentLinkParams<TData>, DocumentLinkContainer<TData>,
                            DocumentLinkRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions))
                    .AddHandler(TextDocumentNames.DocumentLinkResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink<TData>, DocumentLinkRegistrationOptions, TData>(
                            id,
                            resolveHandler,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink<TData>(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams<TData>, IObserver<IEnumerable<DocumentLink<TData>>>, DocumentLinkCapability, CancellationToken> handler,
            DocumentLinkRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                _ => new LanguageProtocolDelegatingHandlers.PartialResults<DocumentLinkParams<TData>, DocumentLinkContainer<TData>, DocumentLink<TData>, DocumentLinkCapability,
                    DocumentLinkRegistrationOptions>(
                    handler,
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    x => new DocumentLinkContainer<TData>(x)));
        }

        public static ILanguageServerRegistry OnDocumentLink<TData>(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams<TData>, IObserver<IEnumerable<DocumentLink<TData>>>, DocumentLinkCapability, CancellationToken> handler,
            Func<DocumentLink<TData>, DocumentLinkCapability, CancellationToken, Task<DocumentLink<TData>>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link, cap, token) => Task.FromException<DocumentLink<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<DocumentLinkParams<TData>, DocumentLinkContainer<TData>, DocumentLink<TData>,
                            DocumentLinkCapability,
                            DocumentLinkRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new DocumentLinkContainer<TData>(x)))
                    .AddHandler(TextDocumentNames.DocumentLinkResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink<TData>, DocumentLinkCapability, DocumentLinkRegistrationOptions, TData>(
                            id,
                            resolveHandler,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink<TData>(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams<TData>, IObserver<IEnumerable<DocumentLink<TData>>>, CancellationToken> handler,
            DocumentLinkRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                _ => new LanguageProtocolDelegatingHandlers.PartialResults<DocumentLinkParams<TData>, DocumentLinkContainer<TData>, DocumentLink<TData>,
                    DocumentLinkRegistrationOptions>(
                    handler,
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    x => new DocumentLinkContainer<TData>(x)));
        }

        public static ILanguageServerRegistry OnDocumentLink<TData>(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams<TData>, IObserver<IEnumerable<DocumentLink<TData>>>, CancellationToken> handler,
            Func<DocumentLink<TData>, CancellationToken, Task<DocumentLink<TData>>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link, token) => Task.FromException<DocumentLink<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<DocumentLinkParams<TData>, DocumentLinkContainer<TData>, DocumentLink<TData>,
                            DocumentLinkRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new DocumentLinkContainer<TData>(x)))
                    .AddHandler(TextDocumentNames.DocumentLinkResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink<TData>, DocumentLinkRegistrationOptions, TData>(
                            id,
                            resolveHandler,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink<TData>(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams<TData>, IObserver<IEnumerable<DocumentLink<TData>>>> handler,
            DocumentLinkRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                _ => new LanguageProtocolDelegatingHandlers.PartialResults<DocumentLinkParams<TData>, DocumentLinkContainer<TData>, DocumentLink<TData>,
                    DocumentLinkRegistrationOptions>(
                    handler,
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    x => new DocumentLinkContainer<TData>(x)));
        }

        public static ILanguageServerRegistry OnDocumentLink<TData>(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams<TData>, IObserver<IEnumerable<DocumentLink<TData>>>> handler,
            Func<DocumentLink<TData>, Task<DocumentLink<TData>>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link) => Task.FromException<DocumentLink<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<DocumentLinkParams<TData>, DocumentLinkContainer<TData>, DocumentLink<TData>,
                            DocumentLinkRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new DocumentLinkContainer<TData>(x)))
                    .AddHandler(TextDocumentNames.DocumentLinkResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink<TData>, DocumentLinkRegistrationOptions, TData>(
                            id,
                            resolveHandler,
                            registrationOptions))
                ;
        }

        public static IRequestProgressObservable<IEnumerable<DocumentLink<ResolvedData>>, DocumentLinkContainer<ResolvedData>> RequestDocumentLink(
            this ITextDocumentLanguageClient mediator,
            DocumentLinkParams<ResolvedData> @params,
            CancellationToken cancellationToken = default)
        {
            return mediator.ProgressManager.MonitorUntil(@params, x => new DocumentLinkContainer<ResolvedData>(x), cancellationToken);
        }

        public static Task<DocumentLink<ResolvedData>> ResolveDocumentLink(this ITextDocumentLanguageClient mediator, DocumentLink<ResolvedData> @params,
            CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }

        public static IRequestProgressObservable<IEnumerable<DocumentLink<TData>>, DocumentLinkContainer<TData>> RequestDocumentLink<TData>(
            this ITextDocumentLanguageClient mediator,
            DocumentLinkParams<TData> @params,
            CancellationToken cancellationToken = default)
            where TData : CanBeResolvedData
        {
            return mediator.ProgressManager.MonitorUntil(@params, x => new DocumentLinkContainer<TData>(x), cancellationToken);
        }

        public static Task<DocumentLink<TData>> ResolveDocumentLink<TData>(this ITextDocumentLanguageClient mediator, DocumentLink<TData> @params,
            CancellationToken cancellationToken = default)
            where TData : CanBeResolvedData
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
