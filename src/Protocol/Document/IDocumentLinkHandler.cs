using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using MediatR;
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
    [Parallel]
    [Method(TextDocumentNames.DocumentLink, Direction.ClientToServer)]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IDocumentLinkHandler : IJsonRpcRequestHandler<DocumentLinkParams, DocumentLinkContainer>,
                                          IRegistration<DocumentLinkRegistrationOptions, DocumentLinkCapability>
    {
    }

    [Parallel]
    [Method(TextDocumentNames.DocumentLinkResolve, Direction.ClientToServer)]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IDocumentLinkResolveHandler : ICanBeResolvedHandler<DocumentLink>, ICanBeIdentifiedHandler
    {
    }

    [Obsolete("This handler is obsolete and is related by DocumentLinkHandlerBase")]
    public abstract class DocumentLinkHandler : AbstractHandlers.Base<DocumentLinkRegistrationOptions, DocumentLinkCapability>, IDocumentLinkHandler
    {
        public abstract Task<DocumentLinkContainer> Handle(DocumentLinkParams request, CancellationToken cancellationToken);
    }

    public abstract class DocumentLinkHandlerBase :
        AbstractHandlers.Base<DocumentLinkRegistrationOptions, DocumentLinkCapability>,
        IDocumentLinkHandler,
        IDocumentLinkResolveHandler
    {
        public abstract Task<DocumentLinkContainer> Handle(DocumentLinkParams request, CancellationToken cancellationToken);
        public abstract Task<DocumentLink> Handle(DocumentLink request, CancellationToken cancellationToken);
        Guid ICanBeIdentifiedHandler.Id { get; } = Guid.NewGuid();
    }

    public abstract class PartialDocumentLinkHandlerBase :
        AbstractHandlers.PartialResults<DocumentLinkParams, DocumentLinkContainer, DocumentLink, DocumentLinkRegistrationOptions, DocumentLinkCapability>, IDocumentLinkHandler, IDocumentLinkResolveHandler
    {
        protected PartialDocumentLinkHandlerBase(IProgressManager progressManager) : base(progressManager, lenses => new DocumentLinkContainer(lenses))
        {
        }

        public abstract Task<DocumentLink> Handle(DocumentLink request, CancellationToken cancellationToken);
        public virtual Guid Id { get; } = Guid.NewGuid();
    }

    public abstract class DocumentLinkHandlerBase<T> : DocumentLinkHandlerBase where T : HandlerIdentity?, new()
    {
        public sealed override async Task<DocumentLinkContainer> Handle(DocumentLinkParams request, CancellationToken cancellationToken)
        {
            var response = await HandleParams(request, cancellationToken).ConfigureAwait(false);
            return response;
        }

        public sealed override async Task<DocumentLink> Handle(DocumentLink request, CancellationToken cancellationToken)
        {
            var response = await HandleResolve(request, cancellationToken).ConfigureAwait(false);
            return response;
        }

        protected abstract Task<DocumentLinkContainer> HandleParams(DocumentLinkParams request, CancellationToken cancellationToken);
        protected abstract Task<DocumentLink<T>> HandleResolve(DocumentLink<T> request, CancellationToken cancellationToken);
    }

    public abstract class PartialDocumentLinkHandlerBase<T> : PartialDocumentLinkHandlerBase where T : HandlerIdentity?, new()
    {
        protected PartialDocumentLinkHandlerBase(IProgressManager progressManager) : base(progressManager)
        {
        }

        protected sealed override void Handle(DocumentLinkParams request, IObserver<IEnumerable<DocumentLink>> results, CancellationToken cancellationToken) => Handle(
            request,
            Observer.Create<IEnumerable<DocumentLink<T>>>(
                x => results.OnNext(x.Select(z => (DocumentLink)z)),
                results.OnError,
                results.OnCompleted
            ), cancellationToken
        );

        public sealed override async Task<DocumentLink> Handle(DocumentLink request, CancellationToken cancellationToken)
        {
            var response = await HandleResolve(request, cancellationToken).ConfigureAwait(false);
            return response;
        }

        protected abstract void Handle(DocumentLinkParams request, IObserver<IEnumerable<DocumentLink<T>>> results, CancellationToken cancellationToken);
        protected abstract Task<DocumentLink<T>> HandleResolve(DocumentLink<T> request, CancellationToken cancellationToken);
    }

    public static partial class DocumentLinkExtensions
    {
        public static ILanguageServerRegistry OnDocumentLink(
            this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, DocumentLinkCapability, CancellationToken, Task<DocumentLinkContainer>> handler,
            Func<DocumentLinkCapability, DocumentLinkRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnDocumentLink(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnDocumentLink(
            this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, DocumentLinkCapability, CancellationToken, Task<DocumentLinkContainer>> handler,
            Func<DocumentLink, DocumentLinkCapability, CancellationToken, Task<DocumentLink>>? resolveHandler,
            Func<DocumentLinkCapability, DocumentLinkRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= _ => new DocumentLinkRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (link, cap, token) => Task.FromResult(link);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.DocumentLink,
                                new LanguageProtocolDelegatingHandlers.Request<DocumentLinkParams, DocumentLinkContainer,
                                    DocumentLinkRegistrationOptions, DocumentLinkCapability>(
                                    id,
                                    handler, registrationOptionsFactory
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.DocumentLinkResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkRegistrationOptions, DocumentLinkCapability>(
                                    id,
                                    resolveHandler, registrationOptionsFactory
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink<T>(
            this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, DocumentLinkCapability, CancellationToken, Task<DocumentLinkContainer<T>>> handler,
            Func<DocumentLink<T>, DocumentLinkCapability, CancellationToken, Task<DocumentLink<T>>>? resolveHandler,
            Func<DocumentLinkCapability, DocumentLinkRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new DocumentLinkRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (link, c, token) => Task.FromResult(link);

            return registry.AddHandler(
                _ => new DelegatingDocumentLinkHandler<T>(
                    registrationOptionsFactory,
                    async (@params, capability, token) => await handler(@params, capability, token),
                    resolveHandler
                )
            );
        }

        public static ILanguageServerRegistry OnDocumentLink(
            this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, CancellationToken, Task<DocumentLinkContainer>> handler,
            Func<DocumentLinkRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnDocumentLink(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnDocumentLink(
            this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, CancellationToken, Task<DocumentLinkContainer>> handler,
            Func<DocumentLink, CancellationToken, Task<DocumentLink>>? resolveHandler,
            Func<DocumentLinkRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new DocumentLinkRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (link, token) => Task.FromResult(link);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.DocumentLink,
                                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentLinkParams, DocumentLinkContainer,
                                    DocumentLinkRegistrationOptions>(
                                    id,
                                    handler, registrationOptionsFactory
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.DocumentLinkResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkRegistrationOptions>(
                                    id,
                                    resolveHandler, registrationOptionsFactory
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink<T>(
            this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, CancellationToken, Task<DocumentLinkContainer<T>>> handler,
            Func<DocumentLink<T>, CancellationToken, Task<DocumentLink<T>>>? resolveHandler,
            Func<DocumentLinkCapability, DocumentLinkRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new DocumentLinkRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (link, token) => Task.FromResult(link);

            return registry.AddHandler(
                _ => new DelegatingDocumentLinkHandler<T>(
                    registrationOptionsFactory,
                    async (@params, capability, token) => await handler(@params, token),
                    (lens, capability, token) => resolveHandler(lens, token)
                )
            );
        }

        public static ILanguageServerRegistry OnDocumentLink(
            this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, Task<DocumentLinkContainer>> handler,
            Func<DocumentLinkRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnDocumentLink(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnDocumentLink(
            this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, Task<DocumentLinkContainer>> handler,
            Func<DocumentLink, Task<DocumentLink>>? resolveHandler,
            Func<DocumentLinkRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new DocumentLinkRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= Task.FromResult;
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.DocumentLink,
                                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentLinkParams, DocumentLinkContainer,
                                    DocumentLinkRegistrationOptions>(
                                    id,
                                    handler, registrationOptionsFactory
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.DocumentLinkResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkRegistrationOptions>(
                                    id,
                                    resolveHandler, registrationOptionsFactory
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink<T>(
            this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, Task<DocumentLinkContainer<T>>> handler,
            Func<DocumentLink<T>, Task<DocumentLink<T>>>? resolveHandler,
            Func<DocumentLinkCapability, DocumentLinkRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new DocumentLinkRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= Task.FromResult;

            return registry.AddHandler(
                _ => new DelegatingDocumentLinkHandler<T>(
                    registrationOptionsFactory,
                    async (@params, capability, token) => await handler(@params),
                    (lens, capability, token) => resolveHandler(lens)
                )
            );
        }

        public static ILanguageServerRegistry OnDocumentLink(
            this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink>>, DocumentLinkCapability, CancellationToken> handler,
            Func<DocumentLinkCapability, DocumentLinkRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnDocumentLink(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnDocumentLink(
            this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink>>, DocumentLinkCapability, CancellationToken> handler,
            Func<DocumentLink, DocumentLinkCapability, CancellationToken, Task<DocumentLink>>? resolveHandler,
            Func<DocumentLinkCapability, DocumentLinkRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= _ => new DocumentLinkRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (lens, capability, token) => Task.FromResult(lens);
            var id = Guid.NewGuid();

            return
                registry.AddHandler(
                             TextDocumentNames.DocumentLink,
                             _ => new DocumentLinkPartialResults(
                                 id,
                                 handler, registrationOptionsFactory,
                                 _.GetRequiredService<IProgressManager>()
                             )
                         )
                        .AddHandler(
                             TextDocumentNames.DocumentLinkResolve,
                             new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkRegistrationOptions, DocumentLinkCapability>(
                                 id,
                                 resolveHandler, registrationOptionsFactory
                             )
                         )
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink<T>(
            this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink<T>>>, DocumentLinkCapability, CancellationToken> handler,
            Func<DocumentLink<T>, DocumentLinkCapability, CancellationToken, Task<DocumentLink<T>>>? resolveHandler,
            Func<DocumentLinkCapability, DocumentLinkRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new DocumentLinkRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (lens, capability, token) => Task.FromResult(lens);

            return registry.AddHandler(
                _ => new DelegatingPartialDocumentLinkHandler<T>(
                    registrationOptionsFactory,
                    _.GetRequiredService<IProgressManager>(),
                    handler,
                    resolveHandler
                )
            );
        }

        public static ILanguageServerRegistry OnDocumentLink(
            this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink>>, CancellationToken> handler,
            Func<DocumentLinkRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnDocumentLink(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnDocumentLink(
            this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink>>, CancellationToken> handler,
            Func<DocumentLink, CancellationToken, Task<DocumentLink>>? resolveHandler,
            Func<DocumentLinkRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new DocumentLinkRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (lens, token) => Task.FromResult(lens);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.DocumentLink,
                                _ => new DocumentLinkPartialResults(
                                    id,
                                    (@params, observer, capability, arg4) => handler(@params, observer, arg4), _ => registrationOptionsFactory(),
                                    _.GetRequiredService<IProgressManager>()
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.DocumentLinkResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptionsFactory
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink<T>(
            this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink<T>>>, CancellationToken> handler,
            Func<DocumentLink<T>, CancellationToken, Task<DocumentLink<T>>>? resolveHandler,
            Func<DocumentLinkCapability, DocumentLinkRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new DocumentLinkRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (lens, token) => Task.FromResult(lens);

            return registry.AddHandler(
                _ => new DelegatingPartialDocumentLinkHandler<T>(
                    registrationOptionsFactory,
                    _.GetRequiredService<IProgressManager>(),
                    (@params, observer, capability, token) => handler(@params, observer, token),
                    (lens, capability, token) => resolveHandler(lens, token)
                )
            );
        }

        public static ILanguageServerRegistry OnDocumentLink(
            this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink>>> handler,
            Func<DocumentLinkRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnDocumentLink(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnDocumentLink(
            this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink>>> handler,
            Func<DocumentLink, Task<DocumentLink>>? resolveHandler,
            Func<DocumentLinkRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new DocumentLinkRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= Task.FromResult;
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.DocumentLink,
                                _ => new DocumentLinkPartialResults(
                                    id,
                                    (@params, observer, _, _) => handler(@params, observer), _ => registrationOptionsFactory(),
                                    _.GetRequiredService<IProgressManager>()
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.DocumentLinkResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptionsFactory
                                )
                            )
                ;
        }

        sealed class DocumentLinkPartialResults :
            AbstractHandlers.PartialResults<DocumentLinkParams, DocumentLinkContainer, DocumentLink, DocumentLinkRegistrationOptions, DocumentLinkCapability>,
            ICanBeIdentifiedHandler
        {
            private readonly Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink>>, DocumentLinkCapability, CancellationToken> _handler;
            private readonly Func<DocumentLinkCapability, DocumentLinkRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public DocumentLinkPartialResults(
                Guid id,
                Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink>>, DocumentLinkCapability, CancellationToken> handler,
                Func<DocumentLinkCapability, DocumentLinkRegistrationOptions> registrationOptionsFactory,
                IProgressManager progressManager
            ) : base(progressManager, z => new DocumentLinkContainer(z.Select(z => z)))
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            protected override DocumentLinkRegistrationOptions CreateRegistrationOptions(DocumentLinkCapability capability) => _registrationOptionsFactory(capability);

            protected override void Handle(DocumentLinkParams request, IObserver<IEnumerable<DocumentLink>> results, CancellationToken cancellationToken) =>
                _handler(request, Observer.Create<IEnumerable<DocumentLink>>(
                             actions => results.OnNext(actions),
                             results.OnError,
                             results.OnCompleted), Capability, cancellationToken);
        }

        public static ILanguageServerRegistry OnDocumentLink<T>(
            this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink<T>>>> handler,
            Func<DocumentLink<T>, Task<DocumentLink<T>>>? resolveHandler,
            Func<DocumentLinkCapability, DocumentLinkRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new DocumentLinkRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= Task.FromResult;

            return registry.AddHandler(
                _ => new DelegatingPartialDocumentLinkHandler<T>(
                    registrationOptionsFactory,
                    _.GetRequiredService<IProgressManager>(),
                    (@params, observer, capability, token) => handler(@params, observer),
                    (lens, capability, token) => resolveHandler(lens)
                )
            );
        }

        private class DelegatingDocumentLinkHandler<T> : DocumentLinkHandlerBase<T> where T : HandlerIdentity?, new()
        {
            private readonly Func<DocumentLinkCapability, DocumentLinkRegistrationOptions> _registrationOptionsFactory;
            private readonly Func<DocumentLinkParams, DocumentLinkCapability, CancellationToken, Task<DocumentLinkContainer>> _handleParams;
            private readonly Func<DocumentLink<T>, DocumentLinkCapability, CancellationToken, Task<DocumentLink<T>>> _handleResolve;

            public DelegatingDocumentLinkHandler(
                Func<DocumentLinkCapability, DocumentLinkRegistrationOptions> registrationOptionsFactory,
                Func<DocumentLinkParams, DocumentLinkCapability, CancellationToken, Task<DocumentLinkContainer>> handleParams,
                Func<DocumentLink<T>, DocumentLinkCapability, CancellationToken, Task<DocumentLink<T>>> handleResolve
            ) : base()
            {
                _registrationOptionsFactory = registrationOptionsFactory;
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override Task<DocumentLinkContainer> HandleParams(DocumentLinkParams request, CancellationToken cancellationToken) =>
                _handleParams(request, Capability, cancellationToken);

            protected override Task<DocumentLink<T>> HandleResolve(DocumentLink<T> request, CancellationToken cancellationToken) => _handleResolve(request, Capability, cancellationToken);
            protected override DocumentLinkRegistrationOptions CreateRegistrationOptions(DocumentLinkCapability capability) => _registrationOptionsFactory(capability);
        }

        private class DelegatingPartialDocumentLinkHandler<T> : PartialDocumentLinkHandlerBase<T> where T : HandlerIdentity?, new()
        {
            private readonly Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink<T>>>, DocumentLinkCapability, CancellationToken> _handleParams;
            private readonly Func<DocumentLink<T>, DocumentLinkCapability, CancellationToken, Task<DocumentLink<T>>> _handleResolve;
            private readonly Func<DocumentLinkCapability, DocumentLinkRegistrationOptions> _registrationOptionsFactory;

            public DelegatingPartialDocumentLinkHandler(
                Func<DocumentLinkCapability, DocumentLinkRegistrationOptions> registrationOptionsFactory,
                IProgressManager progressManager,
                Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink<T>>>, DocumentLinkCapability, CancellationToken> handleParams,
                Func<DocumentLink<T>, DocumentLinkCapability, CancellationToken, Task<DocumentLink<T>>> handleResolve
            ) : base(progressManager)
            {
                _registrationOptionsFactory = registrationOptionsFactory;
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override void Handle(DocumentLinkParams request, IObserver<IEnumerable<DocumentLink<T>>> results, CancellationToken cancellationToken) =>
                _handleParams(request, results, Capability, cancellationToken);

            protected override Task<DocumentLink<T>> HandleResolve(DocumentLink<T> request, CancellationToken cancellationToken) => _handleResolve(request, Capability, cancellationToken);
            protected override DocumentLinkRegistrationOptions CreateRegistrationOptions(DocumentLinkCapability capability) => _registrationOptionsFactory(capability);
        }
    }
}
