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
    [Parallel, Method(TextDocumentNames.CodeLens, Direction.ClientToServer)]
    public interface ICodeLensHandler<TData> : IJsonRpcRequestHandler<CodeLensParams<TData>, CodeLensContainer<TData>>, IRegistration<CodeLensRegistrationOptions>,
        ICapability<CodeLensCapability> where TData : CanBeResolvedData
    {
    }

    [Parallel, Method(TextDocumentNames.CodeLensResolve, Direction.ClientToServer)]
    public interface ICodeLensResolveHandler<TData> : ICanBeResolvedHandler<CodeLens<TData>, TData> where TData : CanBeResolvedData
    {
    }

    public abstract class CodeLensHandler<TData> : ICodeLensHandler<TData>, ICodeLensResolveHandler<TData>
        where TData : CanBeResolvedData
    {
        private readonly CodeLensRegistrationOptions _options;
        private readonly Guid _id = Guid.NewGuid();

        public CodeLensHandler(CodeLensRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public CodeLensRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<CodeLensContainer<TData>> Handle(CodeLensParams<TData> request, CancellationToken cancellationToken);
        public abstract Task<CodeLens<TData>> Handle(CodeLens<TData> request, CancellationToken cancellationToken);
        public virtual void SetCapability(CodeLensCapability capability) => Capability = capability;
        protected CodeLensCapability Capability { get; private set; }
        Guid ICanBeIdentifiedHandler.Id => _id;
    }

    public static class CodeLensExtensions
    {
        public static ILanguageServerRegistry OnCodeLens<TData>(this ILanguageServerRegistry registry,
            Func<CodeLensParams<TData>, CodeLensCapability, CancellationToken, Task<CodeLensContainer<TData>>> handler,
            CodeLensRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CodeLensRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                new LanguageProtocolDelegatingHandlers.Request<CodeLensParams<TData>, CodeLensContainer<TData>, CodeLensCapability,
                    CodeLensRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static ILanguageServerRegistry OnCodeLens<TData>(this ILanguageServerRegistry registry,
            Func<CodeLensParams<TData>, CodeLensCapability, CancellationToken, Task<CodeLensContainer<TData>>> handler,
            Func<CodeLens<TData>, CodeLensCapability, CancellationToken, Task<CodeLens<TData>>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link, cap, token) => Task.FromException<CodeLens<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return registry
                    .AddHandler(TextDocumentNames.CodeLens,
                        new LanguageProtocolDelegatingHandlers.Request<CodeLensParams<TData>, CodeLensContainer<TData>, CodeLensCapability, CodeLensRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions)
                        )
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens<TData>, CodeLensCapability, CodeLensRegistrationOptions, TData>(
                            id,
                            resolveHandler,
                            registrationOptions)
                        )
                ;
        }

        public static ILanguageServerRegistry OnCodeLens<TData>(this ILanguageServerRegistry registry,
            Func<CodeLensParams<TData>, CancellationToken, Task<CodeLensContainer<TData>>> handler,
            CodeLensRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CodeLensRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeLensParams<TData>, CodeLensContainer<TData>,
                    CodeLensRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static ILanguageServerRegistry OnCodeLens<TData>(this ILanguageServerRegistry registry,
            Func<CodeLensParams<TData>, CancellationToken, Task<CodeLensContainer<TData>>> handler,
            Func<CodeLens<TData>, CancellationToken, Task<CodeLens<TData>>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link, token) => Task.FromException<CodeLens<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return registry
                    .AddHandler(TextDocumentNames.CodeLens,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeLensParams<TData>, CodeLensContainer<TData>, CodeLensRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions)
                        )
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens<TData>, CodeLensRegistrationOptions, TData>(
                            id,
                            resolveHandler,
                            registrationOptions)
                        )
                ;
        }

        public static ILanguageServerRegistry OnCodeLens<TData>(this ILanguageServerRegistry registry,
            Func<CodeLensParams<TData>, Task<CodeLensContainer<TData>>> handler,
            CodeLensRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CodeLensRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeLensParams<TData>, CodeLensContainer<TData>,
                    CodeLensRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static ILanguageServerRegistry OnCodeLens<TData>(this ILanguageServerRegistry registry,
            Func<CodeLensParams<TData>, Task<CodeLensContainer<TData>>> handler,
            Func<CodeLens<TData>, Task<CodeLens<TData>>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link) => Task.FromException<CodeLens<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return registry
                    .AddHandler(TextDocumentNames.CodeLens,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeLensParams<TData>, CodeLensContainer<TData>,
                            CodeLensRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions)
                        )
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens<TData>, CodeLensRegistrationOptions, TData>(
                            id,
                            resolveHandler,
                            registrationOptions)
                        )
                ;
        }

        public static ILanguageServerRegistry OnCodeLens<TData>(this ILanguageServerRegistry registry,
            Action<CodeLensParams<TData>, IObserver<IEnumerable<CodeLens<TData>>>, CodeLensCapability, CancellationToken> handler,
            CodeLensRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CodeLensRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                _ => new LanguageProtocolDelegatingHandlers.PartialResults<CodeLensParams<TData>, CodeLensContainer<TData>, CodeLens<TData>, CodeLensCapability,
                    CodeLensRegistrationOptions>(
                    handler,
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    x => new CodeLensContainer<TData>(x)));
        }

        public static ILanguageServerRegistry OnCodeLens<TData>(this ILanguageServerRegistry registry,
            Action<CodeLensParams<TData>, IObserver<IEnumerable<CodeLens<TData>>>, CodeLensCapability, CancellationToken> handler,
            Func<CodeLens<TData>, CodeLensCapability, CancellationToken, Task<CodeLens<TData>>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link, cap, token) => Task.FromException<CodeLens<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return
                registry
                    .AddHandler(TextDocumentNames.CodeLens,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<CodeLensParams<TData>, CodeLensContainer<TData>, CodeLens<TData>, CodeLensCapability, CodeLensRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new CodeLensContainer<TData>(x))
                        )
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens<TData>, CodeLensCapability, CodeLensRegistrationOptions, TData>(
                            id,
                            resolveHandler,
                            registrationOptions)
                        )
                ;
        }

        public static ILanguageServerRegistry OnCodeLens<TData>(this ILanguageServerRegistry registry,
            Action<CodeLensParams<TData>, IObserver<IEnumerable<CodeLens<TData>>>, CancellationToken> handler,
            CodeLensRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CodeLensRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                _ => new LanguageProtocolDelegatingHandlers.PartialResults<CodeLensParams<TData>, CodeLensContainer<TData>, CodeLens<TData>,
                    CodeLensRegistrationOptions>(
                    handler,
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    x => new CodeLensContainer<TData>(x)));
        }

        public static ILanguageServerRegistry OnCodeLens<TData>(this ILanguageServerRegistry registry,
            Action<CodeLensParams<TData>, IObserver<IEnumerable<CodeLens<TData>>>, CancellationToken> handler,
            Func<CodeLens<TData>, CancellationToken, Task<CodeLens<TData>>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link, token) => Task.FromException<CodeLens<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return registry
                    .AddHandler(TextDocumentNames.CodeLens,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<CodeLensParams<TData>, CodeLensContainer<TData>, CodeLens<TData>, CodeLensRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new CodeLensContainer<TData>(x))
                        )
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens<TData>, CodeLensRegistrationOptions, TData>(
                            id,
                            resolveHandler,
                            registrationOptions)
                        )
                ;
        }

        public static ILanguageServerRegistry OnCodeLens<TData>(this ILanguageServerRegistry registry,
            Action<CodeLensParams<TData>, IObserver<IEnumerable<CodeLens<TData>>>> handler,
            CodeLensRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CodeLensRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                _ => new LanguageProtocolDelegatingHandlers.PartialResults<CodeLensParams<TData>, CodeLensContainer<TData>, CodeLens<TData>,
                    CodeLensRegistrationOptions>(
                    handler,
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    x => new CodeLensContainer<TData>(x)));
        }

        public static ILanguageServerRegistry OnCodeLens<TData>(this ILanguageServerRegistry registry,
            Action<CodeLensParams<TData>, IObserver<IEnumerable<CodeLens<TData>>>> handler,
            Func<CodeLens<TData>, Task<CodeLens<TData>>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link) => Task.FromException<CodeLens<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return registry
                    .AddHandler(TextDocumentNames.CodeLens,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<CodeLensParams<TData>, CodeLensContainer<TData>, CodeLens<TData>, CodeLensRegistrationOptions>(
                            id,
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new CodeLensContainer<TData>(x))
                        )
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens<TData>, CodeLensRegistrationOptions, TData>(
                            id,
                            resolveHandler,
                            registrationOptions)
                        )
                ;
        }

        public static IRequestProgressObservable<IEnumerable<CodeLens<ResolvedData>>, CodeLensContainer<ResolvedData>> RequestCodeLens(
            this ITextDocumentLanguageClient mediator,
            CodeLensParams<ResolvedData> @params,
            CancellationToken cancellationToken = default)
        {
            return mediator.ProgressManager.MonitorUntil(@params, x => new CodeLensContainer<ResolvedData>(x), cancellationToken);
        }

        public static IRequestProgressObservable<IEnumerable<CodeLens<TData>>, CodeLensContainer<TData>> RequestCodeLens<TData>(
            this ITextDocumentLanguageClient mediator,
            CodeLensParams<TData> @params,
            CancellationToken cancellationToken = default)
            where TData : CanBeResolvedData
        {
            return mediator.ProgressManager.MonitorUntil(@params, x => new CodeLensContainer<TData>(x), cancellationToken);
        }

        public static Task<CodeLens<ResolvedData>> ResolveCodeLens(this ITextDocumentLanguageClient mediator, CodeLens<ResolvedData> @params,
            CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }

        public static Task<CodeLens<TData>> ResolveCodeLens<TData>(this ITextDocumentLanguageClient mediator, CodeLens<TData> @params,
            CancellationToken cancellationToken = default)
            where TData : CanBeResolvedData
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
