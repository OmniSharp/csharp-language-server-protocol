using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel, Method(TextDocumentNames.Completion, Direction.ClientToServer)]
    public interface ICompletionHandler<TData> : IJsonRpcRequestHandler<CompletionParams<TData>, CompletionList<TData>>, IRegistration<CompletionRegistrationOptions>,
        ICapability<CompletionCapability> where TData : CanBeResolvedData
    {
    }

    [Parallel, Method(TextDocumentNames.CompletionResolve, Direction.ClientToServer)]
    public interface ICompletionResolveHandler<TData> : ICanBeResolvedHandler<CompletionItem<TData>, TData> where TData : CanBeResolvedData
    {
    }

    public abstract class CompletionHandler<TData> : ICompletionHandler<TData>, ICompletionResolveHandler<TData> where TData : CanBeResolvedData
    {
        private readonly CompletionRegistrationOptions _options;
        private readonly Guid _id = Guid.NewGuid();

        public CompletionHandler(CompletionRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public CompletionRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<CompletionList<TData>> Handle(CompletionParams<TData> request, CancellationToken cancellationToken);
        public abstract Task<CompletionItem<TData>> Handle(CompletionItem<TData> request, CancellationToken cancellationToken);
        public virtual void SetCapability(CompletionCapability capability) => Capability = capability;
        protected CompletionCapability Capability { get; private set; }
        Guid ICanBeIdentifiedHandler.Id => _id;
    }

    public static class CompletionExtensions
    {
        public static ILanguageServerRegistry OnCompletion<TData>(
            this ILanguageServerRegistry registry,
            Func<CompletionParams<TData>, CompletionCapability, CancellationToken, Task<CompletionList<TData>>> handler,
            CompletionRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CompletionRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.Completion,
                new LanguageProtocolDelegatingHandlers.Request<CompletionParams<TData>, CompletionList<TData>, CompletionCapability,
                    CompletionRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static ILanguageServerRegistry OnCompletion<TData>(
            this ILanguageServerRegistry registry,
            Func<CompletionParams<TData>, CompletionCapability, CancellationToken, Task<CompletionList<TData>>> handler,
            Func<CompletionItem<TData>, CompletionCapability, CancellationToken, Task<CompletionItem<TData>>> resolveHandler,
            CompletionRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link, cap, token) => Task.FromException<CompletionItem<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return registry
                .AddHandler(TextDocumentNames.Completion,
                    new LanguageProtocolDelegatingHandlers.Request<CompletionParams<TData>, CompletionList<TData>, CompletionCapability, CompletionRegistrationOptions>(
                        id,
                        handler,
                        registrationOptions)
                )
                .AddHandler(TextDocumentNames.CompletionResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem<TData>, CompletionCapability, CompletionRegistrationOptions, TData>(
                        id,
                        resolveHandler,
                        registrationOptions));
        }

        public static ILanguageServerRegistry OnCompletion<TData>(this ILanguageServerRegistry registry,
            Func<CompletionParams<TData>, CancellationToken, Task<CompletionList<TData>>> handler,
            CompletionRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CompletionRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.Completion,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<CompletionParams<TData>, CompletionList<TData>,
                    CompletionRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static ILanguageServerRegistry OnCompletion<TData>(this ILanguageServerRegistry registry,
            Func<CompletionParams<TData>, CancellationToken, Task<CompletionList<TData>>> handler,
            Func<CompletionItem<TData>, CancellationToken, Task<CompletionItem<TData>>> resolveHandler,
            CompletionRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link, token) => Task.FromException<CompletionItem<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return registry
                .AddHandler(TextDocumentNames.Completion,
                    new LanguageProtocolDelegatingHandlers.RequestRegistration<CompletionParams<TData>, CompletionList<TData>, CompletionRegistrationOptions>(
                        id,
                        handler,
                        registrationOptions))
                .AddHandler(TextDocumentNames.CompletionResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem<TData>, CompletionRegistrationOptions, TData>(
                        id,
                        resolveHandler,
                        registrationOptions));
        }

        public static ILanguageServerRegistry OnCompletion<TData>(this ILanguageServerRegistry registry,
            Func<CompletionParams<TData>, Task<CompletionList<TData>>> handler,
            CompletionRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CompletionRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.Completion,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<CompletionParams<TData>, CompletionList<TData>,
                    CompletionRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static ILanguageServerRegistry OnCompletion<TData>(this ILanguageServerRegistry registry,
            Func<CompletionParams<TData>, Task<CompletionList<TData>>> handler,
            Func<CompletionItem<TData>, Task<CompletionItem<TData>>> resolveHandler,
            CompletionRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link) => Task.FromException<CompletionItem<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return registry
                .AddHandler(TextDocumentNames.Completion,
                    new LanguageProtocolDelegatingHandlers.RequestRegistration<CompletionParams<TData>, CompletionList<TData>, CompletionRegistrationOptions>(
                        id,
                        handler,
                        registrationOptions)
                )
                .AddHandler(TextDocumentNames.CompletionResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem<TData>, CompletionRegistrationOptions, TData>(
                        id,
                        resolveHandler,
                        registrationOptions));
        }

        public static ILanguageServerRegistry OnCompletion<TData>(this ILanguageServerRegistry registry,
            Action<CompletionParams<TData>, IObserver<IEnumerable<CompletionItem<TData>>>, CompletionCapability, CancellationToken> handler,
            CompletionRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CompletionRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.Completion,
                _ => new LanguageProtocolDelegatingHandlers.PartialResults<CompletionParams<TData>, CompletionList<TData>, CompletionItem<TData>, CompletionCapability,
                    CompletionRegistrationOptions>(
                    handler,
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    x => new CompletionList<TData>(x)));
        }

        public static ILanguageServerRegistry OnCompletion<TData>(this ILanguageServerRegistry registry,
            Action<CompletionParams<TData>, IObserver<IEnumerable<CompletionItem<TData>>>, CompletionCapability, CancellationToken> handler,
            Func<CompletionItem<TData>, CompletionCapability, CancellationToken, Task<CompletionItem<TData>>> resolveHandler,
            CompletionRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link, cap, token) => Task.FromException<CompletionItem<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.Completion,
                    _ => new LanguageProtocolDelegatingHandlers.PartialResults<CompletionParams<TData>, CompletionList<TData>, CompletionItem<TData>, CompletionCapability, CompletionRegistrationOptions>(
                        id,
                        handler,
                        registrationOptions,
                        _.GetRequiredService<IProgressManager>(),
                        x => new CompletionList<TData>(x))
                )
                .AddHandler(TextDocumentNames.CompletionResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem<TData>, CompletionCapability, CompletionRegistrationOptions, TData>(
                        id,
                        resolveHandler,
                        registrationOptions));
        }

        public static ILanguageServerRegistry OnCompletion<TData>(this ILanguageServerRegistry registry,
            Action<CompletionParams<TData>, IObserver<IEnumerable<CompletionItem<TData>>>, CancellationToken> handler,
            CompletionRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CompletionRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.Completion,
                _ => new LanguageProtocolDelegatingHandlers.PartialResults<CompletionParams<TData>, CompletionList<TData>, CompletionItem<TData>, CompletionRegistrationOptions>(
                    handler,
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    x => new CompletionList<TData>(x)));
        }

        public static ILanguageServerRegistry OnCompletion<TData>(this ILanguageServerRegistry registry,
            Action<CompletionParams<TData>, IObserver<IEnumerable<CompletionItem<TData>>>, CancellationToken> handler,
            Func<CompletionItem<TData>, CancellationToken, Task<CompletionItem<TData>>> resolveHandler,
            CompletionRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link, token) => Task.FromException<CompletionItem<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.Completion,
                    _ => new LanguageProtocolDelegatingHandlers.PartialResults<CompletionParams<TData>, CompletionList<TData>, CompletionItem<TData>, CompletionRegistrationOptions>(
                        id,
                        handler,
                        registrationOptions,
                        _.GetRequiredService<IProgressManager>(),
                        x => new CompletionList<TData>(x)))
                .AddHandler(TextDocumentNames.CompletionResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem<TData>, CompletionRegistrationOptions, TData>(
                        id,
                        resolveHandler,
                        registrationOptions));
        }

        public static ILanguageServerRegistry OnCompletion<TData>(this ILanguageServerRegistry registry,
            Action<CompletionParams<TData>, IObserver<IEnumerable<CompletionItem<TData>>>> handler,
            CompletionRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CompletionRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.Completion,
                _ => new LanguageProtocolDelegatingHandlers.PartialResults<CompletionParams<TData>, CompletionList<TData>, CompletionItem<TData>,
                    CompletionRegistrationOptions>(
                    handler,
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    x => new CompletionList<TData>(x)));
        }

        public static ILanguageServerRegistry OnCompletion<TData>(this ILanguageServerRegistry registry,
            Action<CompletionParams<TData>, IObserver<IEnumerable<CompletionItem<TData>>>> handler,
            Func<CompletionItem<TData>, Task<CompletionItem<TData>>> resolveHandler,
            CompletionRegistrationOptions registrationOptions) where TData : CanBeResolvedData
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = resolveHandler != null;
            resolveHandler ??= (link) => Task.FromException<CompletionItem<TData>>(new NotImplementedException());
            var id = Guid.NewGuid();

            return registry.AddHandler(TextDocumentNames.Completion,
                    _ => new LanguageProtocolDelegatingHandlers.PartialResults<CompletionParams<TData>, CompletionList<TData>, CompletionItem<TData>,
                        CompletionRegistrationOptions>(
                        id,
                        handler,
                        registrationOptions,
                        _.GetRequiredService<IProgressManager>(),
                        x => new CompletionList<TData>(x)))
                .AddHandler(TextDocumentNames.CompletionResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem<TData>, CompletionRegistrationOptions, TData>(
                        id,
                        resolveHandler,
                        registrationOptions));
        }

        public static IRequestProgressObservable<IEnumerable<CompletionItem<ResolvedData>>, CompletionList<ResolvedData>> RequestCompletion(
            this ITextDocumentLanguageClient mediator, CompletionParams<ResolvedData> @params,
            CancellationToken cancellationToken = default)
        {
            return mediator.ProgressManager.MonitorUntil(@params, x => new CompletionList<ResolvedData>(x), cancellationToken);
        }

        public static Task<CompletionItem<ResolvedData>> ResolveCompletion(this ITextDocumentLanguageClient mediator, CompletionItem<ResolvedData> @params,
            CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }

        public static IRequestProgressObservable<IEnumerable<CompletionItem<TData>>, CompletionList<TData>> RequestCompletion<TData>(this ITextDocumentLanguageClient mediator,
            CompletionParams<TData> @params,
            CancellationToken cancellationToken = default)
            where TData : CanBeResolvedData
        {
            return mediator.ProgressManager.MonitorUntil(@params, x => new CompletionList<TData>(x), cancellationToken);
        }

        public static Task<CompletionItem<TData>> ResolveCompletion<TData>(this ITextDocumentLanguageClient mediator, CompletionItem<TData> @params,
            CancellationToken cancellationToken = default)
            where TData : CanBeResolvedData
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
