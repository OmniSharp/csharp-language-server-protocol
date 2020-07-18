using System;
using System.Collections.Generic;
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
    [Parallel, Method(TextDocumentNames.Completion, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICompletionHandler : IJsonRpcRequestHandler<CompletionParams, CompletionList>, IRegistration<CompletionRegistrationOptions>, ICapability<CompletionCapability>
    {
    }

    [Parallel, Method(TextDocumentNames.CompletionResolve, Direction.ClientToServer)]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICompletionResolveHandler : ICanBeResolvedHandler<CompletionItem>
    {
    }

    public abstract class CompletionHandler : ICompletionHandler, ICompletionResolveHandler
    {
        private readonly CompletionRegistrationOptions _options;

        public CompletionHandler(CompletionRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public CompletionRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken);
        public abstract Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken);
        public abstract bool CanResolve(CompletionItem value);
        public virtual void SetCapability(CompletionCapability capability) => Capability = capability;
        protected CompletionCapability Capability { get; private set; }
    }

    public static partial class CompletionExtensions
    {
        public static ILanguageServerRegistry OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, CompletionCapability, CancellationToken, Task<CompletionList>> handler,
            Func<CompletionItem, bool> canResolve,
            Func<CompletionItem, CompletionCapability, CancellationToken, Task<CompletionItem>> resolveHandler,
            CompletionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link, cap, token) => Task.FromException<CompletionItem>(new NotImplementedException());

            return registry
                .AddHandler(TextDocumentNames.Completion,
                    new LanguageProtocolDelegatingHandlers.Request<CompletionParams, CompletionList, CompletionCapability,
                        CompletionRegistrationOptions>(
                        handler,
                        registrationOptions)
                )
                .AddHandler(TextDocumentNames.CompletionResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionCapability, CompletionRegistrationOptions>(
                        resolveHandler,
                        canResolve,
                        registrationOptions));
        }

        public static ILanguageServerRegistry OnCompletion(this ILanguageServerRegistry registry,
            Func<CompletionParams, CancellationToken, Task<CompletionList>> handler,
            Func<CompletionItem, bool> canResolve,
            Func<CompletionItem, CancellationToken, Task<CompletionItem>> resolveHandler,
            CompletionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link, token) => Task.FromException<CompletionItem>(new NotImplementedException());

            return registry
                .AddHandler(TextDocumentNames.Completion,
                    new LanguageProtocolDelegatingHandlers.RequestRegistration<CompletionParams, CompletionList,
                        CompletionRegistrationOptions>(
                        handler,
                        registrationOptions))
                .AddHandler(TextDocumentNames.CompletionResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionRegistrationOptions>(
                        resolveHandler,
                        canResolve,
                        registrationOptions));
        }

        public static ILanguageServerRegistry OnCompletion(this ILanguageServerRegistry registry,
            Func<CompletionParams, Task<CompletionList>> handler,
            Func<CompletionItem, bool> canResolve,
            Func<CompletionItem, Task<CompletionItem>> resolveHandler,
            CompletionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link) => Task.FromException<CompletionItem>(new NotImplementedException());

            return registry
                .AddHandler(TextDocumentNames.Completion,
                    new LanguageProtocolDelegatingHandlers.RequestRegistration<CompletionParams, CompletionList,
                        CompletionRegistrationOptions>(
                        handler,
                        registrationOptions)
                )
                .AddHandler(TextDocumentNames.CompletionResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionRegistrationOptions>(
                        resolveHandler,
                        canResolve,
                        registrationOptions));
        }

        public static ILanguageServerRegistry OnCompletion(this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem>>, CompletionCapability, CancellationToken> handler,
            Func<CompletionItem, bool> canResolve,
            Func<CompletionItem, CompletionCapability, CancellationToken, Task<CompletionItem>> resolveHandler,
            CompletionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link, cap, token) => Task.FromException<CompletionItem>(new NotImplementedException());

            return registry.AddHandler(TextDocumentNames.Completion,
                    _ => new LanguageProtocolDelegatingHandlers.PartialResults<CompletionParams, CompletionList, CompletionItem, CompletionCapability,
                        CompletionRegistrationOptions>(
                        handler,
                        registrationOptions,
                        _.GetRequiredService<IProgressManager>(),
                        x => new CompletionList(x))
                )
                .AddHandler(TextDocumentNames.CompletionResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionCapability, CompletionRegistrationOptions>(
                        resolveHandler,
                        canResolve,
                        registrationOptions));
        }

        public static ILanguageServerRegistry OnCompletion(this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem>>, CancellationToken> handler,
            Func<CompletionItem, bool> canResolve,
            Func<CompletionItem, CancellationToken, Task<CompletionItem>> resolveHandler,
            CompletionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link, token) => Task.FromException<CompletionItem>(new NotImplementedException());

            return registry.AddHandler(TextDocumentNames.Completion,
                    _ => new LanguageProtocolDelegatingHandlers.PartialResults<CompletionParams, CompletionList, CompletionItem,
                        CompletionRegistrationOptions>(
                        handler,
                        registrationOptions,
                        _.GetRequiredService<IProgressManager>(),
                        x => new CompletionList(x)))
                .AddHandler(TextDocumentNames.CompletionResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionRegistrationOptions>(
                        resolveHandler,
                        canResolve,
                        registrationOptions));
        }

        public static ILanguageServerRegistry OnCompletion(this ILanguageServerRegistry registry,
            Action<CompletionParams, IObserver<IEnumerable<CompletionItem>>> handler,
            Func<CompletionItem, bool> canResolve,
            Func<CompletionItem, Task<CompletionItem>> resolveHandler,
            CompletionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link) => Task.FromException<CompletionItem>(new NotImplementedException());

            return registry.AddHandler(TextDocumentNames.Completion,
                    _ => new LanguageProtocolDelegatingHandlers.PartialResults<CompletionParams, CompletionList, CompletionItem,
                        CompletionRegistrationOptions>(
                        handler,
                        registrationOptions,
                        _.GetRequiredService<IProgressManager>(),
                        x => new CompletionList(x)))
                .AddHandler(TextDocumentNames.CompletionResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionRegistrationOptions>(
                        resolveHandler,
                        canResolve,
                        registrationOptions));
        }
    }
}
