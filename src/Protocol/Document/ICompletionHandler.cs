using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
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
    [Parallel]
    [Method(TextDocumentNames.Completion, Direction.ClientToServer)]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICompletionHandler : IJsonRpcRequestHandler<CompletionParams, CompletionList>, IRegistration<CompletionRegistrationOptions>, ICapability<CompletionCapability>
    {
    }

    [Parallel]
    [Method(TextDocumentNames.CompletionResolve, Direction.ClientToServer)]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICompletionResolveHandler : ICanBeResolvedHandler<CompletionItem>
    {
    }

    public abstract class CompletionHandler : ICompletionHandler, ICompletionResolveHandler, ICanBeIdentifiedHandler
    {
        private readonly CompletionRegistrationOptions _options;

        public CompletionHandler(CompletionRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
            _options.ResolveProvider = true;
        }

        public CompletionRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken);
        public abstract Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken);
        Guid ICanBeIdentifiedHandler.Id { get; } = Guid.NewGuid();
        public virtual void SetCapability(CompletionCapability capability) => Capability = capability;
        protected CompletionCapability Capability { get; private set; } = null!;
    }

    public abstract class PartialCompletionHandlerBase :
        AbstractHandlers.PartialResults<CompletionParams, CompletionList, CompletionItem, CompletionCapability, CompletionRegistrationOptions>, ICompletionHandler,
        ICompletionResolveHandler
    {
        protected PartialCompletionHandlerBase(CompletionRegistrationOptions registrationOptions, IProgressManager progressManager) : base(
            registrationOptions, progressManager,
            lenses => new CompletionList(lenses)
        )
        {
        }

        public abstract Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken);
        public virtual Guid Id { get; } = Guid.NewGuid();
    }

    public abstract class CompletionHandlerBase<T> : CompletionHandler where T : HandlerIdentity, new()
    {
        public CompletionHandlerBase(CompletionRegistrationOptions registrationOptions) : base(registrationOptions)
        {
        }

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

        protected abstract Task<CompletionList<T>> HandleParams(CompletionParams request, CancellationToken cancellationToken);
        protected abstract Task<CompletionItem<T>> HandleResolve(CompletionItem<T> request, CancellationToken cancellationToken);
    }

    public abstract class PartialCompletionHandlerBase<T> : PartialCompletionHandlerBase where T : HandlerIdentity, new()
    {
        protected PartialCompletionHandlerBase(CompletionRegistrationOptions registrationOptions, IProgressManager progressManager) : base(
            registrationOptions,
            progressManager
        )
        {
        }

        protected sealed override void Handle(CompletionParams request, IObserver<IEnumerable<CompletionItem>> results, CancellationToken cancellationToken) => Handle(
            request,
            Observer.Create<IEnumerable<CompletionItem<T>>>(
                x => results.OnNext(x.Select(z => (CompletionItem) z)),
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
            CompletionRegistrationOptions? registrationOptions
        ) =>
            OnCompletion(registry, handler, null, registrationOptions);

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, CompletionCapability, CancellationToken, Task<CompletionList>> handler,
            Func<CompletionItem, CompletionCapability, CancellationToken, Task<CompletionItem>>? resolveHandler,
            CompletionRegistrationOptions? registrationOptions
        )
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (link, cap, token) => Task.FromResult(link);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.Completion,
                                new LanguageProtocolDelegatingHandlers.Request<CompletionParams, CompletionList, CompletionCapability,
                                    CompletionRegistrationOptions>(
                                    id,
                                    handler,
                                    registrationOptions
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CompletionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionCapability, CompletionRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptions
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCompletion<T>(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, CompletionCapability, CancellationToken, Task<CompletionList<T>>> handler,
            Func<CompletionItem<T>, CompletionCapability, CancellationToken, Task<CompletionItem<T>>>? resolveHandler,
            CompletionRegistrationOptions? registrationOptions
        ) where T : HandlerIdentity, new()
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (link, c, token) => Task.FromResult(link);

            return registry.AddHandler(
                _ => new DelegatingCompletionHandler<T>(
                    registrationOptions,
                    handler,
                    resolveHandler
                )
            );
        }

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, CancellationToken, Task<CompletionList>> handler,
            CompletionRegistrationOptions? registrationOptions
        ) =>
            OnCompletion(registry, handler, null, registrationOptions);

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, CancellationToken, Task<CompletionList>> handler,
            Func<CompletionItem, CancellationToken, Task<CompletionItem>>? resolveHandler,
            CompletionRegistrationOptions? registrationOptions
        )
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (link, token) => Task.FromResult(link);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.Completion,
                                new LanguageProtocolDelegatingHandlers.RequestRegistration<CompletionParams, CompletionList,
                                    CompletionRegistrationOptions>(
                                    id,
                                    handler,
                                    registrationOptions
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CompletionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptions
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCompletion<T>(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, CancellationToken, Task<CompletionList<T>>> handler,
            Func<CompletionItem<T>, CancellationToken, Task<CompletionItem<T>>>? resolveHandler,
            CompletionRegistrationOptions? registrationOptions
        ) where T : HandlerIdentity, new()
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (link, token) => Task.FromResult(link);

            return registry.AddHandler(
                _ => new DelegatingCompletionHandler<T>(
                    registrationOptions,
                    (@params, capability, token) => handler(@params, token),
                    (lens, capability, token) => resolveHandler(lens, token)
                )
            );
        }

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, Task<CompletionList>> handler,
            CompletionRegistrationOptions? registrationOptions
        ) =>
            OnCompletion(registry, handler, null, registrationOptions);

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, Task<CompletionList>> handler,
            Func<CompletionItem, Task<CompletionItem>>? resolveHandler,
            CompletionRegistrationOptions? registrationOptions
        )
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= Task.FromResult;
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.Completion,
                                new LanguageProtocolDelegatingHandlers.RequestRegistration<CompletionParams, CompletionList,
                                    CompletionRegistrationOptions>(
                                    id,
                                    handler,
                                    registrationOptions
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CompletionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptions
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCompletion<T>(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, Task<CompletionList<T>>> handler,
            Func<CompletionItem<T>, Task<CompletionItem<T>>>? resolveHandler,
            CompletionRegistrationOptions? registrationOptions
        ) where T : HandlerIdentity, new()
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= Task.FromResult;

            return registry.AddHandler(
                _ => new DelegatingCompletionHandler<T>(
                    registrationOptions,
                    (@params, capability, token) => handler(@params),
                    (lens, capability, token) => resolveHandler(lens)
                )
            );
        }

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem>>, CompletionCapability, CancellationToken> handler,
            CompletionRegistrationOptions? registrationOptions
        ) =>
            OnCompletion(registry, handler, null, registrationOptions);

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem>>, CompletionCapability, CancellationToken> handler,
            Func<CompletionItem, CompletionCapability, CancellationToken, Task<CompletionItem>>? resolveHandler,
            CompletionRegistrationOptions? registrationOptions
        )
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (lens, capability, token) => Task.FromResult(lens);
            var id = Guid.NewGuid();

            return
                registry.AddHandler(
                             TextDocumentNames.Completion,
                             _ => new LanguageProtocolDelegatingHandlers.PartialResults<CompletionParams, CompletionList, CompletionItem, CompletionCapability,
                                 CompletionRegistrationOptions>(
                                 id,
                                 handler,
                                 registrationOptions,
                                 _.GetRequiredService<IProgressManager>(),
                                 x => new CompletionList(x)
                             )
                         )
                        .AddHandler(
                             TextDocumentNames.CompletionResolve,
                             new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionCapability, CompletionRegistrationOptions>(
                                 id,
                                 resolveHandler,
                                 registrationOptions
                             )
                         )
                ;
        }

        public static ILanguageServerRegistry OnCompletion<T>(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem<T>>>, CompletionCapability, CancellationToken> handler,
            Func<CompletionItem<T>, CompletionCapability, CancellationToken, Task<CompletionItem<T>>>? resolveHandler,
            CompletionRegistrationOptions? registrationOptions
        ) where T : HandlerIdentity, new()
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (lens, capability, token) => Task.FromResult(lens);

            return registry.AddHandler(
                _ => new DelegatingPartialCompletionHandler<T>(
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    handler,
                    resolveHandler
                )
            );
        }

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem>>, CancellationToken> handler,
            CompletionRegistrationOptions? registrationOptions
        ) =>
            OnCompletion(registry, handler, null, registrationOptions);

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem>>, CancellationToken> handler,
            Func<CompletionItem, CancellationToken, Task<CompletionItem>>? resolveHandler,
            CompletionRegistrationOptions? registrationOptions
        )
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (lens, token) => Task.FromResult(lens);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.Completion,
                                _ => new LanguageProtocolDelegatingHandlers.PartialResults<CompletionParams, CompletionList, CompletionItem,
                                    CompletionRegistrationOptions>(
                                    id,
                                    handler,
                                    registrationOptions,
                                    _.GetRequiredService<IProgressManager>(),
                                    x => new CompletionList(x)
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CompletionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptions
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCompletion<T>(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem<T>>>, CancellationToken> handler,
            Func<CompletionItem<T>, CancellationToken, Task<CompletionItem<T>>>? resolveHandler,
            CompletionRegistrationOptions? registrationOptions
        ) where T : HandlerIdentity, new()
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (lens, token) => Task.FromResult(lens);

            return registry.AddHandler(
                _ => new DelegatingPartialCompletionHandler<T>(
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    (@params, observer, capability, token) => handler(@params, observer, token),
                    (lens, capability, token) => resolveHandler(lens, token)
                )
            );
        }

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem>>> handler,
            CompletionRegistrationOptions? registrationOptions
        ) =>
            OnCompletion(registry, handler, null, registrationOptions);

        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem>>> handler,
            Func<CompletionItem, Task<CompletionItem>>? resolveHandler,
            CompletionRegistrationOptions? registrationOptions
        )
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= Task.FromResult;
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.Completion,
                                _ => new LanguageProtocolDelegatingHandlers.PartialResults<CompletionParams, CompletionList, CompletionItem,
                                    CompletionRegistrationOptions>(
                                    id,
                                    handler,
                                    registrationOptions,
                                    _.GetRequiredService<IProgressManager>(),
                                    x => new CompletionList(x)
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CompletionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptions
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCompletion<T>(
            this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem<T>>>> handler,
            Func<CompletionItem<T>, Task<CompletionItem<T>>>? resolveHandler,
            CompletionRegistrationOptions? registrationOptions
        ) where T : HandlerIdentity, new()
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= Task.FromResult;

            return registry.AddHandler(
                _ => new DelegatingPartialCompletionHandler<T>(
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    (@params, observer, capability, token) => handler(@params, observer),
                    (lens, capability, token) => resolveHandler(lens)
                )
            );
        }

        private class DelegatingCompletionHandler<T> : CompletionHandlerBase<T> where T : HandlerIdentity, new()
        {
            private readonly Func<CompletionParams, CompletionCapability, CancellationToken, Task<CompletionList<T>>> _handleParams;
            private readonly Func<CompletionItem<T>, CompletionCapability, CancellationToken, Task<CompletionItem<T>>> _handleResolve;

            public DelegatingCompletionHandler(
                CompletionRegistrationOptions registrationOptions,
                Func<CompletionParams, CompletionCapability, CancellationToken, Task<CompletionList<T>>> handleParams,
                Func<CompletionItem<T>, CompletionCapability, CancellationToken, Task<CompletionItem<T>>> handleResolve
            ) : base(registrationOptions)
            {
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override Task<CompletionList<T>> HandleParams(CompletionParams request, CancellationToken cancellationToken) =>
                _handleParams(request, Capability, cancellationToken);

            protected override Task<CompletionItem<T>> HandleResolve(CompletionItem<T> request, CancellationToken cancellationToken) =>
                _handleResolve(request, Capability, cancellationToken);
        }

        private class DelegatingPartialCompletionHandler<T> : PartialCompletionHandlerBase<T> where T : HandlerIdentity, new()
        {
            private readonly Action<CompletionParams, IObserver<IEnumerable<CompletionItem<T>>>, CompletionCapability, CancellationToken> _handleParams;
            private readonly Func<CompletionItem<T>, CompletionCapability, CancellationToken, Task<CompletionItem<T>>> _handleResolve;

            public DelegatingPartialCompletionHandler(
                CompletionRegistrationOptions registrationOptions,
                IProgressManager progressManager,
                Action<CompletionParams, IObserver<IEnumerable<CompletionItem<T>>>, CompletionCapability, CancellationToken> handleParams,
                Func<CompletionItem<T>, CompletionCapability, CancellationToken, Task<CompletionItem<T>>> handleResolve
            ) : base(registrationOptions, progressManager)
            {
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override void Handle(CompletionParams request, IObserver<IEnumerable<CompletionItem<T>>> results, CancellationToken cancellationToken) =>
                _handleParams(request, results, Capability, cancellationToken);

            protected override Task<CompletionItem<T>> HandleResolve(CompletionItem<T> request, CancellationToken cancellationToken) =>
                _handleResolve(request, Capability, cancellationToken);
        }
    }
}
