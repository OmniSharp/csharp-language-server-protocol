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
    [Method(TextDocumentNames.CodeLens, Direction.ClientToServer)]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICodeLensHandler : IJsonRpcRequestHandler<CodeLensParams, CodeLensContainer>,
                                          IRegistration<CodeLensRegistrationOptions, CodeLensCapability>
    {
    }

    [Parallel]
    [Method(TextDocumentNames.CodeLensResolve, Direction.ClientToServer)]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICodeLensResolveHandler : ICanBeResolvedHandler<CodeLens>, ICanBeIdentifiedHandler
    {
    }

    [Obsolete("This handler is obsolete and is related by CodeLensHandlerBase")]
    public abstract class CodeLensHandler : AbstractHandlers.Base<CodeLensRegistrationOptions, CodeLensCapability>, ICodeLensHandler
    {
        public abstract Task<CodeLensContainer> Handle(CodeLensParams request, CancellationToken cancellationToken);
    }

    public abstract class CodeLensHandlerBase :
        AbstractHandlers.Base<CodeLensRegistrationOptions, CodeLensCapability>,
        ICodeLensHandler,
        ICodeLensResolveHandler
    {
        public abstract Task<CodeLensContainer> Handle(CodeLensParams request, CancellationToken cancellationToken);
        public abstract Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken);
        Guid ICanBeIdentifiedHandler.Id { get; } = Guid.NewGuid();
    }

    public abstract class PartialCodeLensHandlerBase :
        AbstractHandlers.PartialResults<CodeLensParams, CodeLensContainer, CodeLens, CodeLensRegistrationOptions, CodeLensCapability>, ICodeLensHandler, ICodeLensResolveHandler
    {
        protected PartialCodeLensHandlerBase(IProgressManager progressManager) : base(progressManager, lenses => new CodeLensContainer(lenses))
        {
        }

        public abstract Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken);
        public virtual Guid Id { get; } = Guid.NewGuid();
    }

    public abstract class CodeLensHandlerBase<T> : CodeLensHandlerBase where T : HandlerIdentity?, new()
    {
        public sealed override async Task<CodeLensContainer> Handle(CodeLensParams request, CancellationToken cancellationToken)
        {
            var response = await HandleParams(request, cancellationToken).ConfigureAwait(false);
            return response;
        }

        public sealed override async Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken)
        {
            var response = await HandleResolve(request, cancellationToken).ConfigureAwait(false);
            return response;
        }

        protected abstract Task<CodeLensContainer> HandleParams(CodeLensParams request, CancellationToken cancellationToken);
        protected abstract Task<CodeLens<T>> HandleResolve(CodeLens<T> request, CancellationToken cancellationToken);
    }

    public abstract class PartialCodeLensHandlerBase<T> : PartialCodeLensHandlerBase where T : HandlerIdentity?, new()
    {
        protected PartialCodeLensHandlerBase(IProgressManager progressManager) : base(progressManager)
        {
        }

        protected sealed override void Handle(CodeLensParams request, IObserver<IEnumerable<CodeLens>> results, CancellationToken cancellationToken) => Handle(
            request,
            Observer.Create<IEnumerable<CodeLens<T>>>(
                x => results.OnNext(x.Select(z => (CodeLens)z)),
                results.OnError,
                results.OnCompleted
            ), cancellationToken
        );

        public sealed override async Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken)
        {
            var response = await HandleResolve(request, cancellationToken).ConfigureAwait(false);
            return response;
        }

        protected abstract void Handle(CodeLensParams request, IObserver<IEnumerable<CodeLens<T>>> results, CancellationToken cancellationToken);
        protected abstract Task<CodeLens<T>> HandleResolve(CodeLens<T> request, CancellationToken cancellationToken);
    }

    public static partial class CodeLensExtensions
    {
        public static ILanguageServerRegistry OnCodeLens(
            this ILanguageServerRegistry registry,
            Func<CodeLensParams, CodeLensCapability, CancellationToken, Task<CodeLensContainer>> handler,
            Func<CodeLensCapability, CodeLensRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCodeLens(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCodeLens(
            this ILanguageServerRegistry registry,
            Func<CodeLensParams, CodeLensCapability, CancellationToken, Task<CodeLensContainer>> handler,
            Func<CodeLens, CodeLensCapability, CancellationToken, Task<CodeLens>>? resolveHandler,
            Func<CodeLensCapability, CodeLensRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= _ => new CodeLensRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (link, cap, token) => Task.FromResult(link);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.CodeLens,
                                new LanguageProtocolDelegatingHandlers.Request<CodeLensParams, CodeLensContainer,
                                    CodeLensRegistrationOptions, CodeLensCapability>(
                                    id,
                                    handler, registrationOptionsFactory
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CodeLensResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensRegistrationOptions, CodeLensCapability>(
                                    id,
                                    resolveHandler, registrationOptionsFactory
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCodeLens<T>(
            this ILanguageServerRegistry registry,
            Func<CodeLensParams, CodeLensCapability, CancellationToken, Task<CodeLensContainer<T>>> handler,
            Func<CodeLens<T>, CodeLensCapability, CancellationToken, Task<CodeLens<T>>>? resolveHandler,
            Func<CodeLensCapability, CodeLensRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CodeLensRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (link, c, token) => Task.FromResult(link);

            return registry.AddHandler(
                _ => new DelegatingCodeLensHandler<T>(
                    registrationOptionsFactory,
                    async (@params, capability, token) => await handler(@params, capability, token),
                    resolveHandler
                )
            );
        }

        public static ILanguageServerRegistry OnCodeLens(
            this ILanguageServerRegistry registry,
            Func<CodeLensParams, CancellationToken, Task<CodeLensContainer>> handler,
            Func<CodeLensRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCodeLens(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCodeLens(
            this ILanguageServerRegistry registry,
            Func<CodeLensParams, CancellationToken, Task<CodeLensContainer>> handler,
            Func<CodeLens, CancellationToken, Task<CodeLens>>? resolveHandler,
            Func<CodeLensRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new CodeLensRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (link, token) => Task.FromResult(link);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.CodeLens,
                                new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeLensParams, CodeLensContainer,
                                    CodeLensRegistrationOptions>(
                                    id,
                                    handler, registrationOptionsFactory
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CodeLensResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensRegistrationOptions>(
                                    id,
                                    resolveHandler, registrationOptionsFactory
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCodeLens<T>(
            this ILanguageServerRegistry registry,
            Func<CodeLensParams, CancellationToken, Task<CodeLensContainer<T>>> handler,
            Func<CodeLens<T>, CancellationToken, Task<CodeLens<T>>>? resolveHandler,
            Func<CodeLensCapability, CodeLensRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CodeLensRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (link, token) => Task.FromResult(link);

            return registry.AddHandler(
                _ => new DelegatingCodeLensHandler<T>(
                    registrationOptionsFactory,
                    async (@params, capability, token) => await handler(@params, token),
                    (lens, capability, token) => resolveHandler(lens, token)
                )
            );
        }

        public static ILanguageServerRegistry OnCodeLens(
            this ILanguageServerRegistry registry,
            Func<CodeLensParams, Task<CodeLensContainer>> handler,
            Func<CodeLensRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCodeLens(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCodeLens(
            this ILanguageServerRegistry registry,
            Func<CodeLensParams, Task<CodeLensContainer>> handler,
            Func<CodeLens, Task<CodeLens>>? resolveHandler,
            Func<CodeLensRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new CodeLensRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= Task.FromResult;
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.CodeLens,
                                new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeLensParams, CodeLensContainer,
                                    CodeLensRegistrationOptions>(
                                    id,
                                    handler, registrationOptionsFactory
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CodeLensResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensRegistrationOptions>(
                                    id,
                                    resolveHandler, registrationOptionsFactory
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCodeLens<T>(
            this ILanguageServerRegistry registry,
            Func<CodeLensParams, Task<CodeLensContainer<T>>> handler,
            Func<CodeLens<T>, Task<CodeLens<T>>>? resolveHandler,
            Func<CodeLensCapability, CodeLensRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CodeLensRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= Task.FromResult;

            return registry.AddHandler(
                _ => new DelegatingCodeLensHandler<T>(
                    registrationOptionsFactory,
                    async (@params, capability, token) => await handler(@params),
                    (lens, capability, token) => resolveHandler(lens)
                )
            );
        }

        public static ILanguageServerRegistry OnCodeLens(
            this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>, CodeLensCapability, CancellationToken> handler,
            Func<CodeLensCapability, CodeLensRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCodeLens(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCodeLens(
            this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>, CodeLensCapability, CancellationToken> handler,
            Func<CodeLens, CodeLensCapability, CancellationToken, Task<CodeLens>>? resolveHandler,
            Func<CodeLensCapability, CodeLensRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= _ => new CodeLensRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (lens, capability, token) => Task.FromResult(lens);
            var id = Guid.NewGuid();

            return
                registry.AddHandler(
                             TextDocumentNames.CodeLens,
                             _ => new CodeLensPartialResults(
                                 id,
                                 handler, registrationOptionsFactory,
                                 _.GetRequiredService<IProgressManager>()
                             )
                         )
                        .AddHandler(
                             TextDocumentNames.CodeLensResolve,
                             new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensRegistrationOptions, CodeLensCapability>(
                                 id,
                                 resolveHandler, registrationOptionsFactory
                             )
                         )
                ;
        }

        public static ILanguageServerRegistry OnCodeLens<T>(
            this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens<T>>>, CodeLensCapability, CancellationToken> handler,
            Func<CodeLens<T>, CodeLensCapability, CancellationToken, Task<CodeLens<T>>>? resolveHandler,
            Func<CodeLensCapability, CodeLensRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CodeLensRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (lens, capability, token) => Task.FromResult(lens);

            return registry.AddHandler(
                _ => new DelegatingPartialCodeLensHandler<T>(
                    registrationOptionsFactory,
                    _.GetRequiredService<IProgressManager>(),
                    handler,
                    resolveHandler
                )
            );
        }

        public static ILanguageServerRegistry OnCodeLens(
            this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>, CancellationToken> handler,
            Func<CodeLensRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCodeLens(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCodeLens(
            this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>, CancellationToken> handler,
            Func<CodeLens, CancellationToken, Task<CodeLens>>? resolveHandler,
            Func<CodeLensRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new CodeLensRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (lens, token) => Task.FromResult(lens);
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.CodeLens,
                                _ => new CodeLensPartialResults(
                                    id,
                                    (@params, observer, capability, arg4) => handler(@params, observer, arg4), _ => registrationOptionsFactory(),
                                    _.GetRequiredService<IProgressManager>()
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CodeLensResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptionsFactory
                                )
                            )
                ;
        }

        public static ILanguageServerRegistry OnCodeLens<T>(
            this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens<T>>>, CancellationToken> handler,
            Func<CodeLens<T>, CancellationToken, Task<CodeLens<T>>>? resolveHandler,
            Func<CodeLensCapability, CodeLensRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CodeLensRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= (lens, token) => Task.FromResult(lens);

            return registry.AddHandler(
                _ => new DelegatingPartialCodeLensHandler<T>(
                    registrationOptionsFactory,
                    _.GetRequiredService<IProgressManager>(),
                    (@params, observer, capability, token) => handler(@params, observer, token),
                    (lens, capability, token) => resolveHandler(lens, token)
                )
            );
        }

        public static ILanguageServerRegistry OnCodeLens(
            this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>> handler,
            Func<CodeLensRegistrationOptions>? registrationOptionsFactory
        ) =>
            OnCodeLens(registry, handler, null, registrationOptionsFactory);

        public static ILanguageServerRegistry OnCodeLens(
            this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>> handler,
            Func<CodeLens, Task<CodeLens>>? resolveHandler,
            Func<CodeLensRegistrationOptions>? registrationOptionsFactory
        )
        {
            registrationOptionsFactory ??= () => new CodeLensRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= Task.FromResult;
            var id = Guid.NewGuid();

            return registry.AddHandler(
                                TextDocumentNames.CodeLens,
                                _ => new CodeLensPartialResults(
                                    id,
                                    (@params, observer, _, _) => handler(@params, observer), _ => registrationOptionsFactory(),
                                    _.GetRequiredService<IProgressManager>()
                                )
                            )
                           .AddHandler(
                                TextDocumentNames.CodeLensResolve,
                                new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensRegistrationOptions>(
                                    id,
                                    resolveHandler,
                                    registrationOptionsFactory
                                )
                            )
                ;
        }

        sealed class CodeLensPartialResults :
            AbstractHandlers.PartialResults<CodeLensParams, CodeLensContainer, CodeLens, CodeLensRegistrationOptions, CodeLensCapability>,
            ICanBeIdentifiedHandler
        {
            private readonly Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>, CodeLensCapability, CancellationToken> _handler;
            private readonly Func<CodeLensCapability, CodeLensRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public CodeLensPartialResults(
                Guid id,
                Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>, CodeLensCapability, CancellationToken> handler,
                Func<CodeLensCapability, CodeLensRegistrationOptions> registrationOptionsFactory,
                IProgressManager progressManager
            ) : base(progressManager, z => new CodeLensContainer(z))
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            protected override CodeLensRegistrationOptions CreateRegistrationOptions(CodeLensCapability capability) => _registrationOptionsFactory(capability);

            protected override void Handle(CodeLensParams request, IObserver<IEnumerable<CodeLens>> results, CancellationToken cancellationToken) =>
                _handler(request, results, Capability, cancellationToken);
        }

        public static ILanguageServerRegistry OnCodeLens<T>(
            this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens<T>>>> handler,
            Func<CodeLens<T>, Task<CodeLens<T>>>? resolveHandler,
            Func<CodeLensCapability, CodeLensRegistrationOptions>? registrationOptionsFactory
        ) where T : HandlerIdentity?, new()
        {
            registrationOptionsFactory ??= _ => new CodeLensRegistrationOptions() { ResolveProvider = true };
            resolveHandler ??= Task.FromResult;

            return registry.AddHandler(
                _ => new DelegatingPartialCodeLensHandler<T>(
                    registrationOptionsFactory,
                    _.GetRequiredService<IProgressManager>(),
                    (@params, observer, capability, token) => handler(@params, observer),
                    (lens, capability, token) => resolveHandler(lens)
                )
            );
        }

        private class DelegatingCodeLensHandler<T> : CodeLensHandlerBase<T> where T : HandlerIdentity?, new()
        {
            private readonly Func<CodeLensCapability, CodeLensRegistrationOptions> _registrationOptionsFactory;
            private readonly Func<CodeLensParams, CodeLensCapability, CancellationToken, Task<CodeLensContainer>> _handleParams;
            private readonly Func<CodeLens<T>, CodeLensCapability, CancellationToken, Task<CodeLens<T>>> _handleResolve;

            public DelegatingCodeLensHandler(
                Func<CodeLensCapability, CodeLensRegistrationOptions> registrationOptionsFactory,
                Func<CodeLensParams, CodeLensCapability, CancellationToken, Task<CodeLensContainer>> handleParams,
                Func<CodeLens<T>, CodeLensCapability, CancellationToken, Task<CodeLens<T>>> handleResolve
            ) : base()
            {
                _registrationOptionsFactory = registrationOptionsFactory;
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override Task<CodeLensContainer> HandleParams(CodeLensParams request, CancellationToken cancellationToken) =>
                _handleParams(request, Capability, cancellationToken);

            protected override Task<CodeLens<T>> HandleResolve(CodeLens<T> request, CancellationToken cancellationToken) => _handleResolve(request, Capability, cancellationToken);
            protected override CodeLensRegistrationOptions CreateRegistrationOptions(CodeLensCapability capability) => _registrationOptionsFactory(capability);
        }

        private class DelegatingPartialCodeLensHandler<T> : PartialCodeLensHandlerBase<T> where T : HandlerIdentity?, new()
        {
            private readonly Action<CodeLensParams, IObserver<IEnumerable<CodeLens<T>>>, CodeLensCapability, CancellationToken> _handleParams;
            private readonly Func<CodeLens<T>, CodeLensCapability, CancellationToken, Task<CodeLens<T>>> _handleResolve;
            private readonly Func<CodeLensCapability, CodeLensRegistrationOptions> _registrationOptionsFactory;

            public DelegatingPartialCodeLensHandler(
                Func<CodeLensCapability, CodeLensRegistrationOptions> registrationOptionsFactory,
                IProgressManager progressManager,
                Action<CodeLensParams, IObserver<IEnumerable<CodeLens<T>>>, CodeLensCapability, CancellationToken> handleParams,
                Func<CodeLens<T>, CodeLensCapability, CancellationToken, Task<CodeLens<T>>> handleResolve
            ) : base(progressManager)
            {
                _registrationOptionsFactory = registrationOptionsFactory;
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override void Handle(CodeLensParams request, IObserver<IEnumerable<CodeLens<T>>> results, CancellationToken cancellationToken) =>
                _handleParams(request, results, Capability, cancellationToken);

            protected override Task<CodeLens<T>> HandleResolve(CodeLens<T> request, CancellationToken cancellationToken) => _handleResolve(request, Capability, cancellationToken);
            protected override CodeLensRegistrationOptions CreateRegistrationOptions(CodeLensCapability capability) => _registrationOptionsFactory(capability);
        }
    }
}
