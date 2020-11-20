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
    [Method(TextDocumentNames.Completion, Direction.ClientToServer)]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICompletionHandler : IJsonRpcRequestHandler<CompletionParams, CompletionList>,
                                          IRegistration<CompletionRegistrationOptions, CompletionCapability>
    {
    }

    [Parallel]
    [Method(TextDocumentNames.CompletionResolve, Direction.ClientToServer)]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICompletionResolveHandler : ICanBeResolvedHandler<CompletionItem>, ICanBeIdentifiedHandler
    {
    }

    [Obsolete("This handler is obsolete and is related by CompletionHandlerBase")]
    public abstract class CompletionHandler : AbstractHandlers.Base<CompletionRegistrationOptions, CompletionCapability>, ICompletionHandler
    {
        public abstract Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken);
    }

    public abstract class CompletionHandlerBase :
        AbstractHandlers.Base<CompletionRegistrationOptions, CompletionCapability>,
        ICompletionHandler,
        ICompletionResolveHandler
    {
        public abstract Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken);
        public abstract Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken);
        Guid ICanBeIdentifiedHandler.Id { get; } = Guid.NewGuid();
    }

    public abstract class PartialCompletionHandlerBase :
        AbstractHandlers.PartialResults<CompletionParams, CompletionList, CompletionItem, CompletionRegistrationOptions, CompletionCapability>, ICompletionHandler, ICompletionResolveHandler
    {
        protected PartialCompletionHandlerBase(IProgressManager progressManager) : base(progressManager, lenses => new CompletionList(lenses))
        {
        }

        public abstract Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken);
        public virtual Guid Id { get; } = Guid.NewGuid();
    }

    public abstract class CompletionHandlerBase<T> : CompletionHandlerBase where T : HandlerIdentity?, new()
    {
        public sealed override async Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            var response = await HandleParams(request, cancellationToken).ConfigureAwait(false);
            return response;
        }

        public sealed override async Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken)
        {
            var response = await HandleResolve(request, cancellationToken).ConfigureAwait(false);
            return response;
        }

        protected abstract Task<CompletionList> HandleParams(CompletionParams request, CancellationToken cancellationToken);
        protected abstract Task<CompletionItem<T>> HandleResolve(CompletionItem<T> request, CancellationToken cancellationToken);
    }

    public abstract class PartialCompletionHandlerBase<T> : PartialCompletionHandlerBase where T : HandlerIdentity?, new()
    {
        protected PartialCompletionHandlerBase(IProgressManager progressManager) : base(progressManager)
        {
        }

        protected sealed override void Handle(CompletionParams request, IObserver<IEnumerable<CompletionItem>> results, CancellationToken cancellationToken) => Handle(
            request,
            Observer.Create<IEnumerable<CompletionItem<T>>>(
                x => results.OnNext(x.Select(z => (CompletionItem)z)),
                results.OnError,
                results.OnCompleted
            ), cancellationToken
        );

        public sealed override async Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken)
        {
            var response = await HandleResolve(request, cancellationToken).ConfigureAwait(false);
            return response;
        }

        protected abstract void Handle(CompletionParams request, IObserver<IEnumerable<CompletionItem<T>>> results, CancellationToken cancellationToken);
        protected abstract Task<CompletionItem<T>> HandleResolve(CompletionItem<T> request, CancellationToken cancellationToken);
    }

    public static partial class CompletionExtensions
    {
        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, CompletionCapability, CancellationToken, Task<CompletionList>> handler,
            Func<CompletionCapability, CompletionRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCompletion(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, CompletionCapability, CancellationToken, Task<CompletionList>> handler,
            Func<CompletionItem, CompletionCapability, CancellationToken, Task<CompletionItem>>? resolveHandler,
            Func<CompletionCapability, CompletionRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= _ => new CompletionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (link, cap, token) => Task.FromResult(link);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.Completion,
                                new LanguageProtocolDelegatingHandlers.Request<CompletionParams, CompletionList,
                                    CompletionRegistrationOptions, CompletionCapability>(
                                    id,
                                    handler, registrationOptionsFactory
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CompletionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionRegistrationOptions, CompletionCapability>(
                                    id,
                                    resolveHandler, registrationOptionsFactory
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCompletion<T>(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, CompletionCapability, CancellationToken, Task<CompletionList<T>>> handler,
            Func<CompletionItem<T>, CompletionCapability, CancellationToken, Task<CompletionItem<T>>>? resolveHandler,
            Func<CompletionCapability, CompletionRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CompletionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (link, c, token) => Task.FromResult(link);

            return registry.AddHandler(
                _ => new DelegatingCompletionHandler<T>(
                    registrationOptionsFactory,
                    async (@params, capability, token) => await handler(@params, capability, token),
                    resolveHandler
                )
            );
        }

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, CancellationToken, Task<CompletionList>> handler,
            Func<CompletionRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCompletion(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, CancellationToken, Task<CompletionList>> handler,
            Func<CompletionItem, CancellationToken, Task<CompletionItem>>? resolveHandler,
            Func<CompletionRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new CompletionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (link, token) => Task.FromResult(link);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.Completion,
                                new LanguageProtocolDelegatingHandlers.RequestRegistration<CompletionParams, CompletionList,
                                    CompletionRegistrationOptions>(
                                    id,
                                    handler, registrationOptionsFactory
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CompletionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionRegistrationOptions>(
                                    id,
                                    resolveHandler, registrationOptionsFactory
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCompletion<T>(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, CancellationToken, Task<CompletionList<T>>> handler,
            Func<CompletionItem<T>, CancellationToken, Task<CompletionItem<T>>>? resolveHandler,
            Func<CompletionCapability, CompletionRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CompletionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (link, token) => Task.FromResult(link);

            return registry.AddHandler(
                _ => new DelegatingCompletionHandler<T>(
                    registrationOptionsFactory,
                    async (@params, capability, token) => await handler(@params, token),
                    (lens, capability, token) => resolveHandler(lens, token)
                )
            );
        }

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, Task<CompletionList>> handler,
            Func<CompletionRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCompletion(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, Task<CompletionList>> handler,
            Func<CompletionItem, Task<CompletionItem>>? resolveHandler,
            Func<CompletionRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new CompletionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= Task.FromResult;
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.Completion,
                                new LanguageProtocolDelegatingHandlers.RequestRegistration<CompletionParams, CompletionList,
                                    CompletionRegistrationOptions>(
                                    id,
                                    handler, registrationOptionsFactory
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CompletionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionRegistrationOptions>(
                                    id,
                                    resolveHandler, registrationOptionsFactory
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCompletion<T>(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, Task<CompletionList<T>>> handler,
            Func<CompletionItem<T>, Task<CompletionItem<T>>>? resolveHandler,
            Func<CompletionCapability, CompletionRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CompletionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= Task.FromResult;

            return registry.AddHandler(
                _ => new DelegatingCompletionHandler<T>(
                    registrationOptionsFactory,
                    async (@params, capability, token) => await handler(@params),
                    (lens, capability, token) => resolveHandler(lens)
                )
            );
        }

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem>>, CompletionCapability, CancellationToken> handler,
            Func<CompletionCapability, CompletionRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCompletion(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem>>, CompletionCapability, CancellationToken> handler,
            Func<CompletionItem, CompletionCapability, CancellationToken, Task<CompletionItem>>? resolveHandler,
            Func<CompletionCapability, CompletionRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= _ => new CompletionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (lens, capability, token) => Task.FromResult(lens);
            var id = Guid.NewGuid();

            return
                registry.AddHandler(
                             TextDocumentNames.Completion,
                             _ => new CompletionPartialResults(
                                 id,
                                 handler, registrationOptionsFactory,
                                 _.GetRequiredService<IProgressManager>()
                             )
                         )
                        .AddHandler(
                             TextDocumentNames.CompletionResolve,
                             new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionRegistrationOptions, CompletionCapability>(
                                 id,
                                 resolveHandler, registrationOptionsFactory
                             )
                         )
                ;
        }

        public static ILanguageServerRegistry OnCompletion<T>(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem<T>>>, CompletionCapability, CancellationToken> handler,
            Func<CompletionItem<T>, CompletionCapability, CancellationToken, Task<CompletionItem<T>>>? resolveHandler,
            Func<CompletionCapability, CompletionRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CompletionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (lens, capability, token) => Task.FromResult(lens);

            return registry.AddHandler(
                _ => new DelegatingPartialCompletionHandler<T>(
                    registrationOptionsFactory,
                    _.GetRequiredService<IProgressManager>(),
                    handler,
                    resolveHandler
                )
            );
        }

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem>>, CancellationToken> handler,
            Func<CompletionRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCompletion(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem>>, CancellationToken> handler,
            Func<CompletionItem, CancellationToken, Task<CompletionItem>>? resolveHandler,
            Func<CompletionRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new CompletionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (lens, token) => Task.FromResult(lens);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.Completion,
                                _ => new CompletionPartialResults(
                                    id,
                                    (@params, observer, capability, arg4) => handler(@params, observer, arg4), _ => registrationOptionsFactory(),
                                    _.GetRequiredService<IProgressManager>()
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CompletionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptionsFactory
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCompletion<T>(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem<T>>>, CancellationToken> handler,
            Func<CompletionItem<T>, CancellationToken, Task<CompletionItem<T>>>? resolveHandler,
            Func<CompletionCapability, CompletionRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CompletionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (lens, token) => Task.FromResult(lens);

            return registry.AddHandler(
                _ => new DelegatingPartialCompletionHandler<T>(
                    registrationOptionsFactory,
                    _.GetRequiredService<IProgressManager>(),
                    (@params, observer, capability, token) => handler(@params, observer, token),
                    (lens, capability, token) => resolveHandler(lens, token)
                )
            );
        }

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem>>> handler,
            Func<CompletionRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCompletion(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem>>> handler,
            Func<CompletionItem, Task<CompletionItem>>? resolveHandler,
            Func<CompletionRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new CompletionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= Task.FromResult;
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.Completion,
                                _ => new CompletionPartialResults(
                                    id,
                                    (@params, observer, _, _) => handler(@params, observer), _ => registrationOptionsFactory(),
                                    _.GetRequiredService<IProgressManager>()
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CompletionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptionsFactory
                                )
                            )
                ;
        }

        sealed class CompletionPartialResults :
            AbstractHandlers.PartialResults<CompletionParams, CompletionList, CompletionItem, CompletionRegistrationOptions, CompletionCapability>,
            ICanBeIdentifiedHandler
        {
            private readonly Action<CompletionParams, IObserver<IEnumerable<CompletionItem>>, CompletionCapability, CancellationToken> _handler;
            private readonly Func<CompletionCapability, CompletionRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public CompletionPartialResults(
                Guid id,
                Action<CompletionParams, IObserver<IEnumerable<CompletionItem>>, CompletionCapability, CancellationToken> handler,
                Func<CompletionCapability, CompletionRegistrationOptions> registrationOptionsFactory,
                IProgressManager progressManager
            ) : base(progressManager, z => new CompletionList(z.Select(z => z)))
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            protected override CompletionRegistrationOptions CreateRegistrationOptions(CompletionCapability capability) => _registrationOptionsFactory(capability);

            protected override void Handle(CompletionParams request, IObserver<IEnumerable<CompletionItem>> results, CancellationToken cancellationToken) =>
                _handler(request, Observer.Create<IEnumerable<CompletionItem>>(
                             actions => results.OnNext(actions.Select(z => z)),
                             results.OnError,
                             results.OnCompleted), Capability, cancellationToken);
        }

        public static ILanguageServerRegistry OnCompletion<T>(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem<T>>>> handler,
            Func<CompletionItem<T>, Task<CompletionItem<T>>>? resolveHandler,
            Func<CompletionCapability, CompletionRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CompletionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= Task.FromResult;

            return registry.AddHandler(
                _ => new DelegatingPartialCompletionHandler<T>(
                    registrationOptionsFactory,
                    _.GetRequiredService<IProgressManager>(),
                    (@params, observer, capability, token) => handler(@params, observer),
                    (lens, capability, token) => resolveHandler(lens)
                )
            );
        }

        private class DelegatingCompletionHandler<T> : CompletionHandlerBase<T> where T : HandlerIdentity?, new()
        {
            private readonly Func<CompletionCapability, CompletionRegistrationOptions> _registrationOptionsFactory;
            private readonly Func<CompletionParams, CompletionCapability, CancellationToken, Task<CompletionList>> _handleParams;
            private readonly Func<CompletionItem<T>, CompletionCapability, CancellationToken, Task<CompletionItem<T>>> _handleResolve;

            public DelegatingCompletionHandler(
                Func<CompletionCapability, CompletionRegistrationOptions> registrationOptionsFactory,
                Func<CompletionParams, CompletionCapability, CancellationToken, Task<CompletionList>> handleParams,
                Func<CompletionItem<T>, CompletionCapability, CancellationToken, Task<CompletionItem<T>>> handleResolve
            ) : base()
            {
                _registrationOptionsFactory = registrationOptionsFactory;
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override Task<CompletionList> HandleParams(CompletionParams request, CancellationToken cancellationToken) =>
                _handleParams(request, Capability, cancellationToken);

            protected override Task<CompletionItem<T>> HandleResolve(CompletionItem<T> request, CancellationToken cancellationToken) => _handleResolve(request, Capability, cancellationToken);
            protected override CompletionRegistrationOptions CreateRegistrationOptions(CompletionCapability capability) => _registrationOptionsFactory(capability);
        }

        private class DelegatingPartialCompletionHandler<T> : PartialCompletionHandlerBase<T> where T : HandlerIdentity?, new()
        {
            private readonly Action<CompletionParams, IObserver<IEnumerable<CompletionItem<T>>>, CompletionCapability, CancellationToken> _handleParams;
            private readonly Func<CompletionItem<T>, CompletionCapability, CancellationToken, Task<CompletionItem<T>>> _handleResolve;
            private readonly Func<CompletionCapability, CompletionRegistrationOptions> _registrationOptionsFactory;

            public DelegatingPartialCompletionHandler(
                Func<CompletionCapability, CompletionRegistrationOptions> registrationOptionsFactory,
                IProgressManager progressManager,
                Action<CompletionParams, IObserver<IEnumerable<CompletionItem<T>>>, CompletionCapability, CancellationToken> handleParams,
                Func<CompletionItem<T>, CompletionCapability, CancellationToken, Task<CompletionItem<T>>> handleResolve
            ) : base(progressManager)
            {
                _registrationOptionsFactory = registrationOptionsFactory;
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override void Handle(CompletionParams request, IObserver<IEnumerable<CompletionItem<T>>> results, CancellationToken cancellationToken) =>
                _handleParams(request, results, Capability, cancellationToken);

            protected override Task<CompletionItem<T>> HandleResolve(CompletionItem<T> request, CancellationToken cancellationToken) => _handleResolve(request, Capability, cancellationToken);
            protected override CompletionRegistrationOptions CreateRegistrationOptions(CompletionCapability capability) => _registrationOptionsFactory(capability);
        }
    }
}
