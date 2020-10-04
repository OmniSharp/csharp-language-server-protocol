using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
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
                                          IRegistration<CodeActionRegistrationOptions>, ICapability<CodeActionCapability>
    {
    }

    [Parallel]
    [Method(TextDocumentNames.CodeActionResolve, Direction.ClientToServer)]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICodeActionResolveHandler : ICanBeResolvedHandler<CodeAction>, ICanBeIdentifiedHandler
    {
    }

    [Obsolete("This handler is obsolete and is related by CodeActionHandlerBase")]
    public abstract class CodeActionHandler : ICodeActionHandler
    {
        private readonly CodeActionRegistrationOptions _options;
        public CodeActionHandler(CodeActionRegistrationOptions registrationOptions) => _options = registrationOptions;
        public CodeActionRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<CommandOrCodeActionContainer> Handle(CodeActionParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(CodeActionCapability capability) => Capability = capability;
        protected CodeActionCapability Capability { get; private set; } = null!;
    }

    public abstract class CodeActionHandlerBase : ICodeActionHandler, ICodeActionResolveHandler
    {
        private readonly CodeActionRegistrationOptions _options;

        public CodeActionHandlerBase(CodeActionRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
            _options.ResolveProvider = true;
        }

        public CodeActionRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<CommandOrCodeActionContainer> Handle(CodeActionParams request, CancellationToken cancellationToken);
        public abstract Task<CodeAction> Handle(CodeAction request, CancellationToken cancellationToken);
        Guid ICanBeIdentifiedHandler.Id { get; } = Guid.NewGuid();
        public virtual void SetCapability(CodeActionCapability capability) => Capability = capability;
        protected CodeActionCapability Capability { get; private set; } = null!;
    }

    public abstract class PartialCodeActionHandlerBase :
        AbstractHandlers.PartialResults<CodeActionParams, CommandOrCodeActionContainer, CommandOrCodeAction, CodeActionCapability, CodeActionRegistrationOptions>, ICodeActionHandler, ICodeActionResolveHandler
    {
        protected PartialCodeActionHandlerBase(CodeActionRegistrationOptions registrationOptions, IProgressManager progressManager) : base(
            registrationOptions, progressManager,
            lenses => new CommandOrCodeActionContainer(lenses)
        )
        {
        }

        public abstract Task<CodeAction> Handle(CodeAction request, CancellationToken cancellationToken);
        public virtual Guid Id { get; } = Guid.NewGuid();
    }

    public abstract class CodeActionHandlerBase<T> : CodeActionHandlerBase where T : HandlerIdentity?, new()
    {
        public CodeActionHandlerBase(CodeActionRegistrationOptions registrationOptions) : base(registrationOptions)
        {
        }


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
        protected PartialCodeActionHandlerBase(CodeActionRegistrationOptions registrationOptions, IProgressManager progressManager) : base(
            registrationOptions,
            progressManager
        )
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
            CodeActionRegistrationOptions? registrationOptions
        ) =>
            OnCodeAction(registry, handler, null, registrationOptions);

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CodeActionCapability, CancellationToken, Task<CommandOrCodeActionContainer>> handler,
            Func<CodeAction, CodeActionCapability, CancellationToken, Task<CodeAction>>? resolveHandler,
            CodeActionRegistrationOptions? registrationOptions
        )
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (link, cap, token) => Task.FromResult(link);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.CodeAction,
                                new LanguageProtocolDelegatingHandlers.Request<CodeActionParams, CommandOrCodeActionContainer, CodeActionCapability,
                                    CodeActionRegistrationOptions>(
                                    id,
                                    handler,
                                    registrationOptions
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CodeActionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeAction, CodeActionCapability, CodeActionRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptions
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCodeAction<T>(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CodeActionCapability, CancellationToken, Task<CodeActionContainer<T>>> handler,
            Func<CodeAction<T>, CodeActionCapability, CancellationToken, Task<CodeAction<T>>>? resolveHandler,
            CodeActionRegistrationOptions? registrationOptions
        ) where T : HandlerIdentity?, new()
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (link, c, token) => Task.FromResult(link);

            return registry.AddHandler(
                _ => new DelegatingCodeActionHandler<T>(
                    registrationOptions,
                    async (@params, capability, token) => await handler(@params, capability, token),
                    resolveHandler
                )
            );
        }

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>> handler,
            CodeActionRegistrationOptions? registrationOptions
        ) =>
            OnCodeAction(registry, handler, null, registrationOptions);

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>> handler,
            Func<CodeAction, CancellationToken, Task<CodeAction>>? resolveHandler,
            CodeActionRegistrationOptions? registrationOptions
        )
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (link, token) => Task.FromResult(link);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.CodeAction,
                                new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeActionParams, CommandOrCodeActionContainer,
                                    CodeActionRegistrationOptions>(
                                    id,
                                    handler,
                                    registrationOptions
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CodeActionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeAction, CodeActionRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptions
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCodeAction<T>(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CancellationToken, Task<CodeActionContainer<T>>> handler,
            Func<CodeAction<T>, CancellationToken, Task<CodeAction<T>>>? resolveHandler,
            CodeActionRegistrationOptions? registrationOptions
        ) where T : HandlerIdentity?, new()
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (link, token) => Task.FromResult(link);

            return registry.AddHandler(
                _ => new DelegatingCodeActionHandler<T>(
                    registrationOptions,
                    async (@params, capability, token) => await handler(@params, token),
                    (lens, capability, token) => resolveHandler(lens, token)
                )
            );
        }

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, Task<CommandOrCodeActionContainer>> handler,
            CodeActionRegistrationOptions? registrationOptions
        ) =>
            OnCodeAction(registry, handler, null, registrationOptions);

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, Task<CommandOrCodeActionContainer>> handler,
            Func<CodeAction, Task<CodeAction>>? resolveHandler,
            CodeActionRegistrationOptions? registrationOptions
        )
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= Task.FromResult;
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.CodeAction,
                                new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeActionParams, CommandOrCodeActionContainer,
                                    CodeActionRegistrationOptions>(
                                    id,
                                    handler,
                                    registrationOptions
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CodeActionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeAction, CodeActionRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptions
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCodeAction<T>(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, Task<CodeActionContainer<T>>> handler,
            Func<CodeAction<T>, Task<CodeAction<T>>>? resolveHandler,
            CodeActionRegistrationOptions? registrationOptions
        ) where T : HandlerIdentity?, new()
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= Task.FromResult;

            return registry.AddHandler(
                _ => new DelegatingCodeActionHandler<T>(
                    registrationOptions,
                    async (@params, capability, token) => await handler(@params),
                    (lens, capability, token) => resolveHandler(lens)
                )
            );
        }

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction>>, CodeActionCapability, CancellationToken> handler,
            CodeActionRegistrationOptions? registrationOptions
        ) =>
            OnCodeAction(registry, handler, null, registrationOptions);

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction>>, CodeActionCapability, CancellationToken> handler,
            Func<CodeAction, CodeActionCapability, CancellationToken, Task<CodeAction>>? resolveHandler,
            CodeActionRegistrationOptions? registrationOptions
        )
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (lens, capability, token) => Task.FromResult(lens);
            var id = Guid.NewGuid();

            return
                registry.AddHandler(
                             TextDocumentNames.CodeAction,
                             _ => new CodeActionPartialResults(
                                 id,
                                 handler,
                                 registrationOptions,
                                 _.GetRequiredService<IProgressManager>()
                             )
                         )
                        .AddHandler(
                             TextDocumentNames.CodeActionResolve,
                             new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeAction, CodeActionCapability, CodeActionRegistrationOptions>(
                                 id,
                                 resolveHandler,
                                 registrationOptions
                             )
                         )
                ;
        }

        public static ILanguageServerRegistry OnCodeAction<T>(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction<T>>>, CodeActionCapability, CancellationToken> handler,
            Func<CodeAction<T>, CodeActionCapability, CancellationToken, Task<CodeAction<T>>>? resolveHandler,
            CodeActionRegistrationOptions? registrationOptions
        ) where T : HandlerIdentity?, new()
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (lens, capability, token) => Task.FromResult(lens);

            return registry.AddHandler(
                _ => new DelegatingPartialCodeActionHandler<T>(
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    handler,
                    resolveHandler
                )
            );
        }

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction>>, CancellationToken> handler,
            CodeActionRegistrationOptions? registrationOptions
        ) =>
            OnCodeAction(registry, handler, null, registrationOptions);

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction>>, CancellationToken> handler,
            Func<CodeAction, CancellationToken, Task<CodeAction>>? resolveHandler,
            CodeActionRegistrationOptions? registrationOptions
        )
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (lens, token) => Task.FromResult(lens);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.CodeAction,
                                _ => new CodeActionPartialResults(
                                    id,
                                    (@params, observer, capability, arg4) => handler(@params, observer, arg4),
                                    registrationOptions,
                                    _.GetRequiredService<IProgressManager>()
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CodeActionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeAction, CodeActionRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptions
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCodeAction<T>(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction<T>>>, CancellationToken> handler,
            Func<CodeAction<T>, CancellationToken, Task<CodeAction<T>>>? resolveHandler,
            CodeActionRegistrationOptions? registrationOptions
        ) where T : HandlerIdentity?, new()
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (lens, token) => Task.FromResult(lens);

            return registry.AddHandler(
                _ => new DelegatingPartialCodeActionHandler<T>(
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    (@params, observer, capability, token) => handler(@params, observer, token),
                    (lens, capability, token) => resolveHandler(lens, token)
                )
            );
        }

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction>>> handler,
            CodeActionRegistrationOptions? registrationOptions
        ) =>
            OnCodeAction(registry, handler, null, registrationOptions);

        public static ILanguageServerRegistry OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction>>> handler,
            Func<CodeAction, Task<CodeAction>>? resolveHandler,
            CodeActionRegistrationOptions? registrationOptions
        )
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= Task.FromResult;
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.CodeAction,
                                _ => new CodeActionPartialResults(
                                    id,
                                    (@params, observer, capability, arg3) => handler(@params, observer),
                                    registrationOptions,
                                    _.GetRequiredService<IProgressManager>()
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CodeActionResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeAction, CodeActionRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptions
                                )
                            )
                ;
        }

        sealed class CodeActionPartialResults :
            IJsonRpcRequestHandler<CodeActionParams, CommandOrCodeActionContainer>,
            IRegistration<CodeActionRegistrationOptions>,
            ICanBeIdentifiedHandler,
            ICapability<CodeActionCapability>
        {
            private readonly Action<CodeActionParams, IObserver<IEnumerable<CodeAction>>, CodeActionCapability, CancellationToken> _handler;
            private readonly CodeActionRegistrationOptions _registrationOptions;
            private readonly IProgressManager _progressManager;
            private readonly Guid _id;
            private CodeActionCapability? _capability;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public CodeActionPartialResults(
                Guid id, Action<CodeActionParams, IObserver<IEnumerable<CodeAction>>, CodeActionCapability, CancellationToken> handler, CodeActionRegistrationOptions registrationOptions, IProgressManager progressManager
            )
            {
                _id = id;
                _handler = handler;
                _registrationOptions = registrationOptions;
                _progressManager = progressManager;
            }

            async Task<CommandOrCodeActionContainer> IRequestHandler<CodeActionParams, CommandOrCodeActionContainer>.Handle(CodeActionParams request, CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != ProgressObserver<IEnumerable<CommandOrCodeAction>>.Noop)
                {
                    _handler(
                        request,
                        Observer.Create<IEnumerable<CodeAction>>(
                            v => observer.OnNext(v.Select(z => new CommandOrCodeAction(z))),
                            observer.OnError,
                            observer.OnCompleted
                        ),
                        _capability!,
                        cancellationToken
                        );
                    await observer;
                    return new CommandOrCodeActionContainer();
                }

                var subject = new Subject<IEnumerable<CodeAction>>();
                var task = subject.Aggregate(
                                       new List<CodeAction>(), (acc, items) => {
                                           acc.AddRange(items);
                                           return acc;
                                       }
                                   )
                                  .ToTask(cancellationToken);
                _handler(request, subject, _capability!, cancellationToken);
                var actions = await task.ConfigureAwait(false);
                var result = new CommandOrCodeActionContainer(actions.Select(z => new CommandOrCodeAction(z)));
                return result;
            }

            CodeActionRegistrationOptions IRegistration<CodeActionRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            public void SetCapability(CodeActionCapability capability) => _capability = capability;
        }

        public static ILanguageServerRegistry OnCodeAction<T>(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CodeAction<T>>>> handler,
            Func<CodeAction<T>, Task<CodeAction<T>>>? resolveHandler,
            CodeActionRegistrationOptions? registrationOptions
        ) where T : HandlerIdentity?, new()
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= Task.FromResult;

            return registry.AddHandler(
                _ => new DelegatingPartialCodeActionHandler<T>(
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    (@params, observer, capability, token) => handler(@params, observer),
                    (lens, capability, token) => resolveHandler(lens)
                )
            );
        }

        private class DelegatingCodeActionHandler<T> : CodeActionHandlerBase<T> where T : HandlerIdentity?, new()
        {
            private readonly Func<CodeActionParams, CodeActionCapability, CancellationToken, Task<CommandOrCodeActionContainer>> _handleParams;
            private readonly Func<CodeAction<T>, CodeActionCapability, CancellationToken, Task<CodeAction<T>>> _handleResolve;

            public DelegatingCodeActionHandler(
                CodeActionRegistrationOptions registrationOptions,
                Func<CodeActionParams, CodeActionCapability, CancellationToken, Task<CommandOrCodeActionContainer>> handleParams,
                Func<CodeAction<T>, CodeActionCapability, CancellationToken, Task<CodeAction<T>>> handleResolve
            ) : base(registrationOptions)
            {
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override Task<CommandOrCodeActionContainer> HandleParams(CodeActionParams request, CancellationToken cancellationToken) =>
                _handleParams(request, Capability, cancellationToken);

            protected override Task<CodeAction<T>> HandleResolve(CodeAction<T> request, CancellationToken cancellationToken) => _handleResolve(request, Capability, cancellationToken);
        }

        private class DelegatingPartialCodeActionHandler<T> : PartialCodeActionHandlerBase<T> where T : HandlerIdentity?, new()
        {
            private readonly Action<CodeActionParams, IObserver<IEnumerable<CodeAction<T>>>, CodeActionCapability, CancellationToken> _handleParams;
            private readonly Func<CodeAction<T>, CodeActionCapability, CancellationToken, Task<CodeAction<T>>> _handleResolve;

            public DelegatingPartialCodeActionHandler(
                CodeActionRegistrationOptions registrationOptions,
                IProgressManager progressManager,
                Action<CodeActionParams, IObserver<IEnumerable<CodeAction<T>>>, CodeActionCapability, CancellationToken> handleParams,
                Func<CodeAction<T>, CodeActionCapability, CancellationToken, Task<CodeAction<T>>> handleResolve
            ) : base(registrationOptions, progressManager)
            {
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override void Handle(CodeActionParams request, IObserver<IEnumerable<CodeAction<T>>> results, CancellationToken cancellationToken) =>
                _handleParams(request, results, Capability, cancellationToken);

            protected override Task<CodeAction<T>> HandleResolve(CodeAction<T> request, CancellationToken cancellationToken) => _handleResolve(request, Capability, cancellationToken);
        }
    }
}
