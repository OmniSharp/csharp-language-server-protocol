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
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel, Method(TextDocumentNames.DocumentLink, Direction.ClientToServer)]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IDocumentLinkHandler : IJsonRpcRequestHandler<DocumentLinkParams, DocumentLinkContainer>,
        IRegistration<DocumentLinkRegistrationOptions>, ICapability<DocumentLinkCapability>
    {
    }

    [Parallel, Method(TextDocumentNames.DocumentLinkResolve, Direction.ClientToServer)]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IDocumentLinkResolveHandler : ICanBeResolvedHandler<DocumentLink>
    {
    }

    public abstract class DocumentLinkHandler : IDocumentLinkHandler, IDocumentLinkResolveHandler
    {
        private readonly DocumentLinkRegistrationOptions _options;

        public DocumentLinkHandler(DocumentLinkRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
            _options.ResolveProvider = true;
        }

        public DocumentLinkRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<DocumentLinkContainer> Handle(DocumentLinkParams request, CancellationToken cancellationToken);
        public abstract Task<DocumentLink> Handle(DocumentLink request, CancellationToken cancellationToken);
        public virtual Guid Id { get; } = Guid.NewGuid();
        public virtual void SetCapability(DocumentLinkCapability capability) => Capability = capability;
        protected DocumentLinkCapability Capability { get; private set; }
    }

    public abstract class PartialDocumentLinkHandlerBase :
        AbstractHandlers.PartialResults<DocumentLinkParams, DocumentLinkContainer, DocumentLink, DocumentLinkCapability, DocumentLinkRegistrationOptions>, IDocumentLinkHandler, IDocumentLinkResolveHandler
    {
        protected PartialDocumentLinkHandlerBase(DocumentLinkRegistrationOptions registrationOptions, IProgressManager progressManager) : base(registrationOptions, progressManager,
            lenses => new DocumentLinkContainer(lenses))
        {
        }

        public abstract Task<DocumentLink> Handle(DocumentLink request, CancellationToken cancellationToken);
        public virtual Guid Id { get; } = Guid.NewGuid();
    }

    public abstract class DocumentLinkHandlerBase<T> : DocumentLinkHandler where T : class
    {
        private readonly ISerializer _serializer;

        public DocumentLinkHandlerBase(DocumentLinkRegistrationOptions registrationOptions, ISerializer serializer) : base(registrationOptions)
        {
            _serializer = serializer;
        }


        public sealed override async Task<DocumentLinkContainer> Handle(DocumentLinkParams request, CancellationToken cancellationToken)
        {
            var response = await HandleParams(request, cancellationToken);
            return response.Convert(_serializer);
        }

        public sealed override async Task<DocumentLink> Handle(DocumentLink request, CancellationToken cancellationToken)
        {
            var response = await HandleResolve(request.From<T>(_serializer), cancellationToken);
            return response.To(_serializer);
        }

        protected abstract Task<DocumentLinkContainer<T>> HandleParams(DocumentLinkParams request, CancellationToken cancellationToken);
        protected abstract Task<DocumentLink<T>> HandleResolve(DocumentLink<T> request, CancellationToken cancellationToken);
    }

    public abstract class PartialDocumentLinkHandlerBase<T> : PartialDocumentLinkHandlerBase where T : class
    {
        private readonly ISerializer _serializer;

        protected PartialDocumentLinkHandlerBase(DocumentLinkRegistrationOptions registrationOptions, IProgressManager progressManager, ISerializer serializer) : base(registrationOptions,
            progressManager)
        {
            _serializer = serializer;
        }

        protected sealed override void Handle(DocumentLinkParams request, IObserver<IEnumerable<DocumentLink>> results, CancellationToken cancellationToken) => Handle(
            request,
            Observer.Create<IEnumerable<DocumentLink<T>>>(
                x => results.OnNext(x.Select(z => z.To(_serializer))),
                results.OnError,
                results.OnCompleted
            ), cancellationToken);

        public sealed override async Task<DocumentLink> Handle(DocumentLink request, CancellationToken cancellationToken)
        {
            var response = await HandleResolve(request.From<T>(_serializer), cancellationToken);
            return response.To(_serializer);
        }

        protected abstract void Handle(DocumentLinkParams request, IObserver<IEnumerable<DocumentLink<T>>> results, CancellationToken cancellationToken);
        protected abstract Task<DocumentLink<T>> HandleResolve(DocumentLink<T> request, CancellationToken cancellationToken);
    }

    public static partial class DocumentLinkExtensions
    {
        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, DocumentLinkCapability, CancellationToken, Task<DocumentLinkContainer>> handler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            return OnDocumentLink(registry, handler, null, registrationOptions);
        }

        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, DocumentLinkCapability, CancellationToken, Task<DocumentLinkContainer>> handler,
            Func<DocumentLink, DocumentLinkCapability, CancellationToken, Task<DocumentLink>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (link, cap, token) => Task.FromResult(link);
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                        new LanguageProtocolDelegatingHandlers.Request<DocumentLinkParams, DocumentLinkContainer, DocumentLinkCapability,
                            DocumentLinkRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions))
                    .AddHandler(TextDocumentNames.DocumentLinkResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkCapability, DocumentLinkRegistrationOptions>(
                            id,
                            resolveHandler,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink<T>(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, DocumentLinkCapability, CancellationToken, Task<DocumentLinkContainer<T>>> handler,
            Func<DocumentLink<T>, DocumentLinkCapability, CancellationToken, Task<DocumentLink<T>>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions) where T : class
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (link, c, token) => Task.FromResult(link);

            return registry.AddHandler(_ => new DelegatingDocumentLinkHandler<T>(
                registrationOptions,
                _.GetRequiredService<ISerializer>(),
                handler,
                resolveHandler)
            );
        }

        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, CancellationToken, Task<DocumentLinkContainer>> handler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            return OnDocumentLink(registry, handler, null, registrationOptions);
        }

        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, CancellationToken, Task<DocumentLinkContainer>> handler,
            Func<DocumentLink, CancellationToken, Task<DocumentLink>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (link, token) => Task.FromResult(link);
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentLinkParams, DocumentLinkContainer,
                            DocumentLinkRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions))
                    .AddHandler(TextDocumentNames.DocumentLinkResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkRegistrationOptions>(
                            id,
                            resolveHandler,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink<T>(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, CancellationToken, Task<DocumentLinkContainer<T>>> handler,
            Func<DocumentLink<T>, CancellationToken, Task<DocumentLink<T>>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions) where T : class
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (link, token) => Task.FromResult(link);

            return registry.AddHandler(_ => new DelegatingDocumentLinkHandler<T>(
                registrationOptions,
                _.GetRequiredService<ISerializer>(),
                (@params, capability, token) => handler(@params, token),
                (lens, capability, token) => resolveHandler(lens, token))
            );
        }

        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, Task<DocumentLinkContainer>> handler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            return OnDocumentLink(registry, handler, null, registrationOptions);
        }

        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, Task<DocumentLinkContainer>> handler,
            Func<DocumentLink, Task<DocumentLink>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= Task.FromResult;
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentLinkParams, DocumentLinkContainer,
                            DocumentLinkRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions))
                    .AddHandler(TextDocumentNames.DocumentLinkResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkRegistrationOptions>(
                            id,
                            resolveHandler,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink<T>(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, Task<DocumentLinkContainer<T>>> handler,
            Func<DocumentLink<T>, Task<DocumentLink<T>>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions) where T : class
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= Task.FromResult;

            return registry.AddHandler(_ => new DelegatingDocumentLinkHandler<T>(
                registrationOptions,
                _.GetRequiredService<ISerializer>(),
                (@params, capability, token) => handler(@params),
                (lens, capability, token) => resolveHandler(lens))
            );
        }

        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink>>, DocumentLinkCapability, CancellationToken> handler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            return OnDocumentLink(registry, handler, null, registrationOptions);
        }

        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink>>, DocumentLinkCapability, CancellationToken> handler,
            Func<DocumentLink, DocumentLinkCapability, CancellationToken, Task<DocumentLink>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (lens, capability, token) => Task.FromResult(lens);
            var id = Guid.NewGuid();

            return
                registry.AddHandler(TextDocumentNames.DocumentLink,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<DocumentLinkParams, DocumentLinkContainer, DocumentLink, DocumentLinkCapability,
                            DocumentLinkRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new DocumentLinkContainer(x)))
                    .AddHandler(TextDocumentNames.DocumentLinkResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkCapability, DocumentLinkRegistrationOptions>(
                            id,
                            resolveHandler,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink<T>(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink<T>>>, DocumentLinkCapability, CancellationToken> handler,
            Func<DocumentLink<T>, DocumentLinkCapability, CancellationToken, Task<DocumentLink<T>>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions) where T : class
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (lens, capability, token) => Task.FromResult(lens);

            return registry.AddHandler(_ => new DelegatingPartialDocumentLinkHandler<T>(
                registrationOptions,
                _.GetRequiredService<IProgressManager>(),
                _.GetRequiredService<ISerializer>(),
                handler,
                resolveHandler)
            );
        }

        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink>>, CancellationToken> handler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            return OnDocumentLink(registry, handler, null, registrationOptions);
        }

        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink>>, CancellationToken> handler,
            Func<DocumentLink, CancellationToken, Task<DocumentLink>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (lens, token) => Task.FromResult(lens);
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<DocumentLinkParams, DocumentLinkContainer, DocumentLink,
                            DocumentLinkRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new DocumentLinkContainer(x)))
                    .AddHandler(TextDocumentNames.DocumentLinkResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkRegistrationOptions>(
                            id,
                            resolveHandler,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink<T>(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink<T>>>, CancellationToken> handler,
            Func<DocumentLink<T>, CancellationToken, Task<DocumentLink<T>>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions) where T : class
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (lens,  token) => Task.FromResult(lens);

            return registry.AddHandler(_ => new DelegatingPartialDocumentLinkHandler<T>(
                registrationOptions,
                _.GetRequiredService<IProgressManager>(),
                _.GetRequiredService<ISerializer>(),
                (@params, observer, capability, token) => handler(@params, observer, token),
                (lens, capability, token) => resolveHandler(lens, token))
            );
        }

        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink>>> handler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            return OnDocumentLink(registry, handler, null, registrationOptions);
        }

        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink>>> handler,
            Func<DocumentLink, Task<DocumentLink>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= Task.FromResult;
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<DocumentLinkParams, DocumentLinkContainer, DocumentLink,
                            DocumentLinkRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new DocumentLinkContainer(x)))
                    .AddHandler(TextDocumentNames.DocumentLinkResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkRegistrationOptions>(
                            id,
                            resolveHandler,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink<T>(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink<T>>>> handler,
            Func<DocumentLink<T>, Task<DocumentLink<T>>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions) where T : class
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= Task.FromResult;

            return registry.AddHandler(_ => new DelegatingPartialDocumentLinkHandler<T>(
                registrationOptions,
                _.GetRequiredService<IProgressManager>(),
                _.GetRequiredService<ISerializer>(),
                (@params, observer, capability, token) => handler(@params, observer),
                (lens, capability, token) => resolveHandler(lens))
            );
        }

        class DelegatingDocumentLinkHandler<T> : DocumentLinkHandlerBase<T> where T : class
        {
            private readonly Func<DocumentLinkParams, DocumentLinkCapability, CancellationToken, Task<DocumentLinkContainer<T>>> _handleParams;
            private readonly Func<DocumentLink<T>, DocumentLinkCapability, CancellationToken, Task<DocumentLink<T>>> _handleResolve;

            public DelegatingDocumentLinkHandler(
                DocumentLinkRegistrationOptions registrationOptions,
                ISerializer serializer,
                Func<DocumentLinkParams, DocumentLinkCapability, CancellationToken, Task<DocumentLinkContainer<T>>> handleParams,
                Func<DocumentLink<T>, DocumentLinkCapability, CancellationToken, Task<DocumentLink<T>>> handleResolve
            ) : base(registrationOptions, serializer)
            {
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override Task<DocumentLinkContainer<T>> HandleParams(DocumentLinkParams request, CancellationToken cancellationToken) =>
                _handleParams(request, Capability, cancellationToken);

            protected override Task<DocumentLink<T>> HandleResolve(DocumentLink<T> request, CancellationToken cancellationToken) => _handleResolve(request, Capability, cancellationToken);
        }

        class DelegatingPartialDocumentLinkHandler<T> : PartialDocumentLinkHandlerBase<T> where T : class
        {
            private readonly Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink<T>>>, DocumentLinkCapability, CancellationToken> _handleParams;
            private readonly Func<DocumentLink<T>, DocumentLinkCapability, CancellationToken, Task<DocumentLink<T>>> _handleResolve;

            public DelegatingPartialDocumentLinkHandler(
                DocumentLinkRegistrationOptions registrationOptions,
                IProgressManager progressManager,
                ISerializer serializer,
                Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink<T>>>, DocumentLinkCapability, CancellationToken> handleParams,
                Func<DocumentLink<T>, DocumentLinkCapability, CancellationToken, Task<DocumentLink<T>>> handleResolve
            ) : base(registrationOptions, progressManager, serializer)
            {
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override void Handle(DocumentLinkParams request, IObserver<IEnumerable<DocumentLink<T>>> results, CancellationToken cancellationToken) =>
                _handleParams(request, results, Capability, cancellationToken);

            protected override Task<DocumentLink<T>> HandleResolve(DocumentLink<T> request, CancellationToken cancellationToken) => _handleResolve(request, Capability, cancellationToken);
        }
    }
}
