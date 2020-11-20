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
    [Method(TextDocumentNames.CodeAction, Direction.ClientToServer)]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICodeActionHandler : IJsonRpcRequestHandler<CodeActionParams, CommandOrCodeActionContainer>,
                                          IRegistration<CodeActionRegistrationOptions, CodeActionCapability>
    {
    }

    [Parallel]
    [Method(TextDocumentNames.CodeActionResolve, Direction.ClientToServer)]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICodeActionResolveHandler : ICanBeResolvedHandler<CodeAction>, ICanBeIdentifiedHandler
    {
    }

    [Obsolete("This handler is obsolete and is related by CodeActionHandlerBase")]
    public abstract class CodeActionHandler : AbstractHandlers.Base<CodeActionRegistrationOptions, CodeActionCapability>, ICodeActionHandler
    {
        public abstract Task<CommandOrCodeActionContainer> Handle(CodeActionParams request, CancellationToken cancellationToken);
    }

    public abstract class CodeActionHandlerBase :
        AbstractHandlers.Base<CodeActionRegistrationOptions, CodeActionCapability>,
        ICodeActionHandler,
        ICodeActionResolveHandler
    {
        public abstract Task<CommandOrCodeActionContainer> Handle(CodeActionParams request, CancellationToken cancellationToken);
        public abstract Task<CodeAction> Handle(CodeAction request, CancellationToken cancellationToken);
        Guid ICanBeIdentifiedHandler.Id { get; } = Guid.NewGuid();
    }

    public abstract class PartialCodeActionHandlerBase :
        AbstractHandlers.PartialResults<CodeActionParams, CommandOrCodeActionContainer, CommandOrCodeAction, CodeActionRegistrationOptions, CodeActionCapability>, ICodeActionHandler, ICodeActionResolveHandler
    {
        protected PartialCodeActionHandlerBase(IProgressManager progressManager) : base(progressManager, lenses => new CommandOrCodeActionContainer(lenses))
        {
        }

        public abstract Task<CodeAction> Handle(CodeAction request, CancellationToken cancellationToken);
        public virtual Guid Id { get; } = Guid.NewGuid();
    }

    public abstract class CodeActionHandlerBase<T> : CodeActionHandlerBase where T : HandlerIdentity?, new()
    {
        public sealed override async Task<CommandOrCodeActionContainer> Handle(CodeActionParams request, CancellationToken cancellationToken)
        {
            var response = await HandleParams(request, cancellationToken).ConfigureAwait(false);
            return response;
        }

        public sealed override async Task<CodeAction> Handle(CodeAction request, CancellationToken cancellationToken)
        {
            var response = await HandleResolve(request, cancellationToken).ConfigureAwait(false);
            return response;
        }

        protected abstract Task<CommandOrCodeActionContainer> HandleParams(CodeActionParams request, CancellationToken cancellationToken);
        protected abstract Task<CodeAction<T>> HandleResolve(CodeAction<T> request, CancellationToken cancellationToken);
    }

    public abstract class PartialCodeActionHandlerBase<T> : PartialCodeActionHandlerBase where T : HandlerIdentity?, new()
    {
        protected PartialCodeActionHandlerBase(IProgressManager progressManager) : base(progressManager)
        {
        }

        protected sealed override void Handle(CodeActionParams request, IObserver<IEnumerable<CommandOrCodeAction>> results, CancellationToken cancellationToken) => Handle(
            request,
            Observer.Create<IEnumerable<CodeAction<T>>>(
                x => results.OnNext(x.Select(z => new CommandOrCodeAction(z))),
                results.OnError,
                results.OnCompleted
            ), cancellationToken
        );

        public sealed override async Task<CodeAction> Handle(CodeAction request, CancellationToken cancellationToken)
        {
            var response = await HandleResolve(request, cancellationToken).ConfigureAwait(false);
            return response;
        }

        protected abstract void Handle(CodeActionParams request, IObserver<IEnumerable<CodeAction<T>>> results, CancellationToken cancellationToken);
        protected abstract Task<CodeAction<T>> HandleResolve(CodeAction<T> request, CancellationToken cancellationToken);
    }

    public static partial class CodeActionExtensions
    {
        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CodeActionCapability, CancellationToken, Task<CommandOrCodeActionContainer>> handler,
            Func<CodeActionCapability, CodeActionRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCodeAction(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CodeActionCapability, CancellationToken, Task<CommandOrCodeActionContainer>> handler,
            Func<CodeAction, CodeActionCapability, CancellationToken, Task<CodeAction>>? resolveHandler,
            Func<CodeActionCapability, CodeActionRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= _ => new CodeActionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (link, cap, token) => Task.FromResult(link);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.CodeAction,
                                new LanguageProtocolDelegatingHandlers.Request<CodeActionParams, CommandOrCodeActionContainer,
                                    CodeActionRegistrationOptions, CodeActionCapability>(
                                    id,
                                    handler, registrationOptionsFactory
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CodeActionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeAction, CodeActionRegistrationOptions, CodeActionCapability>(
                                    id,
                                    resolveHandler, registrationOptionsFactory
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCodeAction<T>(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CodeActionCapability, CancellationToken, Task<CodeActionContainer<T>>> handler,
            Func<CodeAction<T>, CodeActionCapability, CancellationToken, Task<CodeAction<T>>>? resolveHandler,
            Func<CodeActionCapability, CodeActionRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CodeActionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (link, c, token) => Task.FromResult(link);

            return registry.AddHandler(
                _ => new DelegatingCodeActionHandler<T>(
                    registrationOptionsFactory,
                    async (@params, capability, token) => await handler(@params, capability, token),
                    resolveHandler
                )
            );
        }

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>> handler,
            Func<CodeActionRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCodeAction(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>> handler,
            Func<CodeAction, CancellationToken, Task<CodeAction>>? resolveHandler,
            Func<CodeActionRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new CodeActionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (link, token) => Task.FromResult(link);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.CodeAction,
                                new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeActionParams, CommandOrCodeActionContainer,
                                    CodeActionRegistrationOptions>(
                                    id,
                                    handler, registrationOptionsFactory
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CodeActionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeAction, CodeActionRegistrationOptions>(
                                    id,
                                    resolveHandler, registrationOptionsFactory
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCodeAction<T>(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CancellationToken, Task<CodeActionContainer<T>>> handler,
            Func<CodeAction<T>, CancellationToken, Task<CodeAction<T>>>? resolveHandler,
            Func<CodeActionCapability, CodeActionRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CodeActionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (link, token) => Task.FromResult(link);

            return registry.AddHandler(
                _ => new DelegatingCodeActionHandler<T>(
                    registrationOptionsFactory,
                    async (@params, capability, token) => await handler(@params, token),
                    (lens, capability, token) => resolveHandler(lens, token)
                )
            );
        }

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, Task<CommandOrCodeActionContainer>> handler,
            Func<CodeActionRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCodeAction(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, Task<CommandOrCodeActionContainer>> handler,
            Func<CodeAction, Task<CodeAction>>? resolveHandler,
            Func<CodeActionRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new CodeActionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= Task.FromResult;
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.CodeAction,
                                new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeActionParams, CommandOrCodeActionContainer,
                                    CodeActionRegistrationOptions>(
                                    id,
                                    handler, registrationOptionsFactory
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CodeActionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeAction, CodeActionRegistrationOptions>(
                                    id,
                                    resolveHandler, registrationOptionsFactory
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCodeAction<T>(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, Task<CodeActionContainer<T>>> handler,
            Func<CodeAction<T>, Task<CodeAction<T>>>? resolveHandler,
            Func<CodeActionCapability, CodeActionRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CodeActionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= Task.FromResult;

            return registry.AddHandler(
                _ => new DelegatingCodeActionHandler<T>(
                    registrationOptionsFactory,
                    async (@params, capability, token) => await handler(@params),
                    (lens, capability, token) => resolveHandler(lens)
                )
            );
        }

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction>>, CodeActionCapability, CancellationToken> handler,
            Func<CodeActionCapability, CodeActionRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCodeAction(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction>>, CodeActionCapability, CancellationToken> handler,
            Func<CodeAction, CodeActionCapability, CancellationToken, Task<CodeAction>>? resolveHandler,
            Func<CodeActionCapability, CodeActionRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= _ => new CodeActionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (lens, capability, token) => Task.FromResult(lens);
            var id = Guid.NewGuid();

            return
                registry.AddHandler(
                             TextDocumentNames.CodeAction,
                             _ => new CodeActionPartialResults(
                                 id,
                                 handler, registrationOptionsFactory,
                                 _.GetRequiredService<IProgressManager>()
                             )
                         )
                        .AddHandler(
                             TextDocumentNames.CodeActionResolve,
                             new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeAction, CodeActionRegistrationOptions, CodeActionCapability>(
                                 id,
                                 resolveHandler, registrationOptionsFactory
                             )
                         )
                ;
        }

        public static ILanguageServerRegistry OnCodeAction<T>(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction<T>>>, CodeActionCapability, CancellationToken> handler,
            Func<CodeAction<T>, CodeActionCapability, CancellationToken, Task<CodeAction<T>>>? resolveHandler,
            Func<CodeActionCapability, CodeActionRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CodeActionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (lens, capability, token) => Task.FromResult(lens);

            return registry.AddHandler(
                _ => new DelegatingPartialCodeActionHandler<T>(
                    registrationOptionsFactory,
                    _.GetRequiredService<IProgressManager>(),
                    handler,
                    resolveHandler
                )
            );
        }

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction>>, CancellationToken> handler,
            Func<CodeActionRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCodeAction(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction>>, CancellationToken> handler,
            Func<CodeAction, CancellationToken, Task<CodeAction>>? resolveHandler,
            Func<CodeActionRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new CodeActionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (lens, token) => Task.FromResult(lens);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.CodeAction,
                                _ => new CodeActionPartialResults(
                                    id,
                                    (@params, observer, capability, arg4) => handler(@params, observer, arg4), _ => registrationOptionsFactory(),
                                    _.GetRequiredService<IProgressManager>()
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CodeActionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeAction, CodeActionRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptionsFactory
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCodeAction<T>(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction<T>>>, CancellationToken> handler,
            Func<CodeAction<T>, CancellationToken, Task<CodeAction<T>>>? resolveHandler,
            Func<CodeActionCapability, CodeActionRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CodeActionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (lens, token) => Task.FromResult(lens);

            return registry.AddHandler(
                _ => new DelegatingPartialCodeActionHandler<T>(
                    registrationOptionsFactory,
                    _.GetRequiredService<IProgressManager>(),
                    (@params, observer, capability, token) => handler(@params, observer, token),
                    (lens, capability, token) => resolveHandler(lens, token)
                )
            );
        }

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction>>> handler,
            Func<CodeActionRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCodeAction(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction>>> handler,
            Func<CodeAction, Task<CodeAction>>? resolveHandler,
            Func<CodeActionRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new CodeActionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= Task.FromResult;
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.CodeAction,
                                _ => new CodeActionPartialResults(
                                    id,
                                    (@params, observer, _, _) => handler(@params, observer), _ => registrationOptionsFactory(),
                                    _.GetRequiredService<IProgressManager>()
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CodeActionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeAction, CodeActionRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptionsFactory
                                )
                            )
                ;
        }

        sealed class CodeActionPartialResults :
            AbstractHandlers.PartialResults<CodeActionParams, CommandOrCodeActionContainer, CommandOrCodeAction, CodeActionRegistrationOptions, CodeActionCapability>,
            ICanBeIdentifiedHandler
        {
            private readonly Action<CodeActionParams, IObserver<IEnumerable<CodeAction>>, CodeActionCapability, CancellationToken> _handler;
            private readonly Func<CodeActionCapability, CodeActionRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public CodeActionPartialResults(
                Guid id,
                Action<CodeActionParams, IObserver<IEnumerable<CodeAction>>, CodeActionCapability, CancellationToken> handler,
                Func<CodeActionCapability, CodeActionRegistrationOptions> registrationOptionsFactory,
                IProgressManager progressManager
            ) : base(progressManager, z => new CommandOrCodeActionContainer(z.Select(z => z)))
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            protected override CodeActionRegistrationOptions CreateRegistrationOptions(CodeActionCapability capability) => _registrationOptionsFactory(capability);

            protected override void Handle(CodeActionParams request, IObserver<IEnumerable<CommandOrCodeAction>> results, CancellationToken cancellationToken) =>
                _handler(request, Observer.Create<IEnumerable<CodeAction>>(
                             actions => results.OnNext(actions.Select(z => new CommandOrCodeAction(z))),
                             results.OnError,
                             results.OnCompleted), Capability, cancellationToken);
        }

        public static ILanguageServerRegistry OnCodeAction<T>(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction<T>>>> handler,
            Func<CodeAction<T>, Task<CodeAction<T>>>? resolveHandler,
            Func<CodeActionCapability, CodeActionRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CodeActionRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= Task.FromResult;

            return registry.AddHandler(
                _ => new DelegatingPartialCodeActionHandler<T>(
                    registrationOptionsFactory,
                    _.GetRequiredService<IProgressManager>(),
                    (@params, observer, capability, token) => handler(@params, observer),
                    (lens, capability, token) => resolveHandler(lens)
                )
            );
        }

        private class DelegatingCodeActionHandler<T> : CodeActionHandlerBase<T> where T : HandlerIdentity?, new()
        {
            private readonly Func<CodeActionCapability, CodeActionRegistrationOptions> _registrationOptionsFactory;
            private readonly Func<CodeActionParams, CodeActionCapability, CancellationToken, Task<CommandOrCodeActionContainer>> _handleParams;
            private readonly Func<CodeAction<T>, CodeActionCapability, CancellationToken, Task<CodeAction<T>>> _handleResolve;

            public DelegatingCodeActionHandler(
                Func<CodeActionCapability, CodeActionRegistrationOptions> registrationOptionsFactory,
                Func<CodeActionParams, CodeActionCapability, CancellationToken, Task<CommandOrCodeActionContainer>> handleParams,
                Func<CodeAction<T>, CodeActionCapability, CancellationToken, Task<CodeAction<T>>> handleResolve
            ) : base()
            {
                _registrationOptionsFactory = registrationOptionsFactory;
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override Task<CommandOrCodeActionContainer> HandleParams(CodeActionParams request, CancellationToken cancellationToken) =>
                _handleParams(request, Capability, cancellationToken);

            protected override Task<CodeAction<T>> HandleResolve(CodeAction<T> request, CancellationToken cancellationToken) => _handleResolve(request, Capability, cancellationToken);
            protected override CodeActionRegistrationOptions CreateRegistrationOptions(CodeActionCapability capability) => _registrationOptionsFactory(capability);
        }

        private class DelegatingPartialCodeActionHandler<T> : PartialCodeActionHandlerBase<T> where T : HandlerIdentity?, new()
        {
            private readonly Action<CodeActionParams, IObserver<IEnumerable<CodeAction<T>>>, CodeActionCapability, CancellationToken> _handleParams;
            private readonly Func<CodeAction<T>, CodeActionCapability, CancellationToken, Task<CodeAction<T>>> _handleResolve;
            private readonly Func<CodeActionCapability, CodeActionRegistrationOptions> _registrationOptionsFactory;

            public DelegatingPartialCodeActionHandler(
                Func<CodeActionCapability, CodeActionRegistrationOptions> registrationOptionsFactory,
                IProgressManager progressManager,
                Action<CodeActionParams, IObserver<IEnumerable<CodeAction<T>>>, CodeActionCapability, CancellationToken> handleParams,
                Func<CodeAction<T>, CodeActionCapability, CancellationToken, Task<CodeAction<T>>> handleResolve
            ) : base(progressManager)
            {
                _registrationOptionsFactory = registrationOptionsFactory;
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override void Handle(CodeActionParams request, IObserver<IEnumerable<CodeAction<T>>> results, CancellationToken cancellationToken) =>
                _handleParams(request, results, Capability, cancellationToken);

            protected override Task<CodeAction<T>> HandleResolve(CodeAction<T> request, CancellationToken cancellationToken) => _handleResolve(request, Capability, cancellationToken);
            protected override CodeActionRegistrationOptions CreateRegistrationOptions(CodeActionCapability capability) => _registrationOptionsFactory(capability);
        }
    }
}
