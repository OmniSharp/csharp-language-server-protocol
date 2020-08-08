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
    [Parallel, Method(TextDocumentNames.CodeLens, Direction.ClientToServer)]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICodeLensHandler : IJsonRpcRequestHandler<CodeLensParams, CodeLensContainer>, IRegistration<CodeLensRegistrationOptions>, ICapability<CodeLensCapability>
    {
    }

    [Parallel, Method(TextDocumentNames.CodeLensResolve, Direction.ClientToServer)]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICodeLensResolveHandler : ICanBeResolvedHandler<CodeLens>
    {
    }

    public abstract class CodeLensHandler : ICodeLensHandler, ICodeLensResolveHandler, ICanBeIdentifiedHandler
    {
        private readonly CodeLensRegistrationOptions _options;

        public CodeLensHandler(CodeLensRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
            _options.ResolveProvider = true;
        }

        public CodeLensRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<CodeLensContainer> Handle(CodeLensParams request, CancellationToken cancellationToken);
        public abstract Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken);
        Guid ICanBeIdentifiedHandler.Id { get; } = Guid.NewGuid();
        public virtual void SetCapability(CodeLensCapability capability) => Capability = capability;
        protected CodeLensCapability Capability { get; private set; }
    }

    public abstract class PartialCodeLensHandlerBase :
        AbstractHandlers.PartialResults<CodeLensParams, CodeLensContainer, CodeLens, CodeLensCapability, CodeLensRegistrationOptions>, ICodeLensHandler, ICodeLensResolveHandler
    {
        protected PartialCodeLensHandlerBase(CodeLensRegistrationOptions registrationOptions, IProgressManager progressManager) : base(registrationOptions, progressManager,
            lenses => new CodeLensContainer(lenses))
        {
        }

        public abstract Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken);
        public virtual Guid Id { get; } = Guid.NewGuid();
    }

    public abstract class CodeLensHandlerBase<T> : CodeLensHandler where T : HandlerIdentity, new()
    {
        private readonly ISerializer _serializer;

        public CodeLensHandlerBase(CodeLensRegistrationOptions registrationOptions, ISerializer serializer) : base(registrationOptions)
        {
            _serializer = serializer;
        }


        public sealed override async Task<CodeLensContainer> Handle(CodeLensParams request, CancellationToken cancellationToken)
        {
            var response = await HandleParams(request, cancellationToken);
            return response;
        }

        public sealed override async Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken)
        {
            var response = await HandleResolve(request, cancellationToken);
            return response;
        }

        protected abstract Task<CodeLensContainer<T>> HandleParams(CodeLensParams request, CancellationToken cancellationToken);
        protected abstract Task<CodeLens<T>> HandleResolve(CodeLens<T> request, CancellationToken cancellationToken);
    }

    public abstract class PartialCodeLensHandlerBase<T> : PartialCodeLensHandlerBase where T : HandlerIdentity, new()
    {
        private readonly ISerializer _serializer;

        protected PartialCodeLensHandlerBase(CodeLensRegistrationOptions registrationOptions, IProgressManager progressManager, ISerializer serializer) : base(registrationOptions,
            progressManager)
        {
            _serializer = serializer;
        }

        protected sealed override void Handle(CodeLensParams request, IObserver<IEnumerable<CodeLens>> results, CancellationToken cancellationToken) => Handle(
            request,
            Observer.Create<IEnumerable<CodeLens<T>>>(
                x => results.OnNext(x.Select(z => (CodeLens)z)),
                results.OnError,
                results.OnCompleted
            ), cancellationToken);

        public sealed override async Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken)
        {
            var response = await HandleResolve(request, cancellationToken);
            return response;
        }

        protected abstract void Handle(CodeLensParams request, IObserver<IEnumerable<CodeLens<T>>> results, CancellationToken cancellationToken);
        protected abstract Task<CodeLens<T>> HandleResolve(CodeLens<T> request, CancellationToken cancellationToken);
    }

    public static partial class CodeLensExtensions
    {
        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Func<CodeLensParams, CodeLensCapability, CancellationToken, Task<CodeLensContainer>> handler,
            CodeLensRegistrationOptions registrationOptions)
        {
            return OnCodeLens(registry, handler, null, registrationOptions);
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Func<CodeLensParams, CodeLensCapability, CancellationToken, Task<CodeLensContainer>> handler,
            Func<CodeLens, CodeLensCapability, CancellationToken, Task<CodeLens>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (link, cap, token) => Task.FromResult(link);
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                        new LanguageProtocolDelegatingHandlers.Request<CodeLensParams, CodeLensContainer, CodeLensCapability,
                            CodeLensRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions))
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensCapability, CodeLensRegistrationOptions>(
                            id,
                            resolveHandler,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnCodeLens<T>(this ILanguageServerRegistry registry,
            Func<CodeLensParams, CodeLensCapability, CancellationToken, Task<CodeLensContainer<T>>> handler,
            Func<CodeLens<T>, CodeLensCapability, CancellationToken, Task<CodeLens<T>>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions) where T : HandlerIdentity, new()
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (link, c, token) => Task.FromResult(link);

            return registry.AddHandler(_ => new DelegatingCodeLensHandler<T>(
                registrationOptions,
                _.GetRequiredService<ISerializer>(),
                handler,
                resolveHandler)
            );
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Func<CodeLensParams, CancellationToken, Task<CodeLensContainer>> handler,
            CodeLensRegistrationOptions registrationOptions)
        {
            return OnCodeLens(registry, handler, null, registrationOptions);
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Func<CodeLensParams, CancellationToken, Task<CodeLensContainer>> handler,
            Func<CodeLens, CancellationToken, Task<CodeLens>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (link, token) => Task.FromResult(link);
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeLensParams, CodeLensContainer,
                            CodeLensRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions))
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensRegistrationOptions>(
                            id,
                            resolveHandler,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnCodeLens<T>(this ILanguageServerRegistry registry,
            Func<CodeLensParams, CancellationToken, Task<CodeLensContainer<T>>> handler,
            Func<CodeLens<T>, CancellationToken, Task<CodeLens<T>>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions) where T : HandlerIdentity, new()
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (link, token) => Task.FromResult(link);

            return registry.AddHandler(_ => new DelegatingCodeLensHandler<T>(
                registrationOptions,
                _.GetRequiredService<ISerializer>(),
                (@params, capability, token) => handler(@params, token),
                (lens, capability, token) => resolveHandler(lens, token))
            );
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Func<CodeLensParams, Task<CodeLensContainer>> handler,
            CodeLensRegistrationOptions registrationOptions)
        {
            return OnCodeLens(registry, handler, null, registrationOptions);
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Func<CodeLensParams, Task<CodeLensContainer>> handler,
            Func<CodeLens, Task<CodeLens>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= Task.FromResult;
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeLensParams, CodeLensContainer,
                            CodeLensRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions))
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensRegistrationOptions>(
                            id,
                            resolveHandler,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnCodeLens<T>(this ILanguageServerRegistry registry,
            Func<CodeLensParams, Task<CodeLensContainer<T>>> handler,
            Func<CodeLens<T>, Task<CodeLens<T>>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions) where T : HandlerIdentity, new()
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= Task.FromResult;

            return registry.AddHandler(_ => new DelegatingCodeLensHandler<T>(
                registrationOptions,
                _.GetRequiredService<ISerializer>(),
                (@params, capability, token) => handler(@params),
                (lens, capability, token) => resolveHandler(lens))
            );
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>, CodeLensCapability, CancellationToken> handler,
            CodeLensRegistrationOptions registrationOptions)
        {
            return OnCodeLens(registry, handler, null, registrationOptions);
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>, CodeLensCapability, CancellationToken> handler,
            Func<CodeLens, CodeLensCapability, CancellationToken, Task<CodeLens>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (lens, capability, token) => Task.FromResult(lens);
            var id = Guid.NewGuid();

            return
                registry.AddHandler(TextDocumentNames.CodeLens,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<CodeLensParams, CodeLensContainer, CodeLens, CodeLensCapability,
                            CodeLensRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new CodeLensContainer(x)))
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensCapability, CodeLensRegistrationOptions>(
                            id,
                            resolveHandler,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnCodeLens<T>(this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens<T>>>, CodeLensCapability, CancellationToken> handler,
            Func<CodeLens<T>, CodeLensCapability, CancellationToken, Task<CodeLens<T>>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions) where T : HandlerIdentity, new()
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (lens, capability, token) => Task.FromResult(lens);

            return registry.AddHandler(_ => new DelegatingPartialCodeLensHandler<T>(
                registrationOptions,
                _.GetRequiredService<IProgressManager>(),
                _.GetRequiredService<ISerializer>(),
                handler,
                resolveHandler)
            );
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>, CancellationToken> handler,
            CodeLensRegistrationOptions registrationOptions)
        {
            return OnCodeLens(registry, handler, null, registrationOptions);
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>, CancellationToken> handler,
            Func<CodeLens, CancellationToken, Task<CodeLens>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (lens, token) => Task.FromResult(lens);
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<CodeLensParams, CodeLensContainer, CodeLens,
                            CodeLensRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new CodeLensContainer(x)))
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensRegistrationOptions>(
                            id,
                            resolveHandler,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnCodeLens<T>(this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens<T>>>, CancellationToken> handler,
            Func<CodeLens<T>, CancellationToken, Task<CodeLens<T>>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions) where T : HandlerIdentity, new()
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= (lens,  token) => Task.FromResult(lens);

            return registry.AddHandler(_ => new DelegatingPartialCodeLensHandler<T>(
                registrationOptions,
                _.GetRequiredService<IProgressManager>(),
                _.GetRequiredService<ISerializer>(),
                (@params, observer, capability, token) => handler(@params, observer, token),
                (lens, capability, token) => resolveHandler(lens, token))
            );
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>> handler,
            CodeLensRegistrationOptions registrationOptions)
        {
            return OnCodeLens(registry, handler, null, registrationOptions);
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>> handler,
            Func<CodeLens, Task<CodeLens>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= Task.FromResult;
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<CodeLensParams, CodeLensContainer, CodeLens,
                            CodeLensRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new CodeLensContainer(x)))
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensRegistrationOptions>(
                            id,
                            resolveHandler,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnCodeLens<T>(this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens<T>>>> handler,
            Func<CodeLens<T>, Task<CodeLens<T>>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions) where T : HandlerIdentity, new()
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = true;
            resolveHandler ??= Task.FromResult;

            return registry.AddHandler(_ => new DelegatingPartialCodeLensHandler<T>(
                registrationOptions,
                _.GetRequiredService<IProgressManager>(),
                _.GetRequiredService<ISerializer>(),
                (@params, observer, capability, token) => handler(@params, observer),
                (lens, capability, token) => resolveHandler(lens))
            );
        }

        class DelegatingCodeLensHandler<T> : CodeLensHandlerBase<T> where T : HandlerIdentity, new()
        {
            private readonly Func<CodeLensParams, CodeLensCapability, CancellationToken, Task<CodeLensContainer<T>>> _handleParams;
            private readonly Func<CodeLens<T>, CodeLensCapability, CancellationToken, Task<CodeLens<T>>> _handleResolve;

            public DelegatingCodeLensHandler(
                CodeLensRegistrationOptions registrationOptions,
                ISerializer serializer,
                Func<CodeLensParams, CodeLensCapability, CancellationToken, Task<CodeLensContainer<T>>> handleParams,
                Func<CodeLens<T>, CodeLensCapability, CancellationToken, Task<CodeLens<T>>> handleResolve
            ) : base(registrationOptions, serializer)
            {
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override Task<CodeLensContainer<T>> HandleParams(CodeLensParams request, CancellationToken cancellationToken) =>
                _handleParams(request, Capability, cancellationToken);

            protected override Task<CodeLens<T>> HandleResolve(CodeLens<T> request, CancellationToken cancellationToken) => _handleResolve(request, Capability, cancellationToken);
        }

        class DelegatingPartialCodeLensHandler<T> : PartialCodeLensHandlerBase<T> where T : HandlerIdentity, new()
        {
            private readonly Action<CodeLensParams, IObserver<IEnumerable<CodeLens<T>>>, CodeLensCapability, CancellationToken> _handleParams;
            private readonly Func<CodeLens<T>, CodeLensCapability, CancellationToken, Task<CodeLens<T>>> _handleResolve;

            public DelegatingPartialCodeLensHandler(
                CodeLensRegistrationOptions registrationOptions,
                IProgressManager progressManager,
                ISerializer serializer,
                Action<CodeLensParams, IObserver<IEnumerable<CodeLens<T>>>, CodeLensCapability, CancellationToken> handleParams,
                Func<CodeLens<T>, CodeLensCapability, CancellationToken, Task<CodeLens<T>>> handleResolve
            ) : base(registrationOptions, progressManager, serializer)
            {
                _handleParams = handleParams;
                _handleResolve = handleResolve;
            }

            protected override void Handle(CodeLensParams request, IObserver<IEnumerable<CodeLens<T>>> results, CancellationToken cancellationToken) =>
                _handleParams(request, results, Capability, cancellationToken);

            protected override Task<CodeLens<T>> HandleResolve(CodeLens<T> request, CancellationToken cancellationToken) => _handleResolve(request, Capability, cancellationToken);
        }
    }
}
