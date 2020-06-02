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
    public interface ICodeLensHandler : IJsonRpcRequestHandler<CodeLensParams, CodeLensContainer>, IRegistration<CodeLensRegistrationOptions>, ICapability<CodeLensCapability>
    {
    }

    [Parallel, Method(TextDocumentNames.CodeLensResolve, Direction.ClientToServer)]
    public interface ICodeLensResolveHandler : ICanBeResolvedHandler<CodeLens>
    {
    }

    public abstract class CodeLensHandler : ICodeLensHandler, ICodeLensResolveHandler
    {
        private readonly CodeLensRegistrationOptions _options;

        public CodeLensHandler(CodeLensRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public CodeLensRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<CodeLensContainer> Handle(CodeLensParams request, CancellationToken cancellationToken);
        public abstract Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken);
        public abstract bool CanResolve(CodeLens value);
        public virtual void SetCapability(CodeLensCapability capability) => Capability = capability;
        protected CodeLensCapability Capability { get; private set; }
    }

    public static class CodeLensExtensions
    {
        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Func<CodeLensParams, CodeLensCapability, CancellationToken, Task<CodeLensContainer>> handler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                new LanguageProtocolDelegatingHandlers.Request<CodeLensParams, CodeLensContainer, CodeLensCapability,
                    CodeLensRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Func<CodeLensParams, CodeLensCapability, CancellationToken, Task<CodeLensContainer>> handler,
            Func<CodeLens, bool> canResolve,
            Func<CodeLens, CodeLensCapability, CancellationToken, Task<CodeLens>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link, cap, token) => Task.FromException<CodeLens>(new NotImplementedException());

            return registry.AddHandler(TextDocumentNames.CodeLens,
                        new LanguageProtocolDelegatingHandlers.Request<CodeLensParams, CodeLensContainer, CodeLensCapability,
                            CodeLensRegistrationOptions>(
                            handler,
                            registrationOptions))
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensCapability, CodeLensRegistrationOptions>(
                            resolveHandler,
                            canResolve,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Func<CodeLensParams, CancellationToken, Task<CodeLensContainer>> handler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeLensParams, CodeLensContainer,
                    CodeLensRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Func<CodeLensParams, CancellationToken, Task<CodeLensContainer>> handler,
            Func<CodeLens, bool> canResolve,
            Func<CodeLens, CancellationToken, Task<CodeLens>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link, token) => Task.FromException<CodeLens>(new NotImplementedException());

            return registry.AddHandler(TextDocumentNames.CodeLens,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeLensParams, CodeLensContainer,
                            CodeLensRegistrationOptions>(
                            handler,
                            registrationOptions))
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensRegistrationOptions>(
                            resolveHandler,
                            canResolve,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Func<CodeLensParams, Task<CodeLensContainer>> handler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeLensParams, CodeLensContainer,
                    CodeLensRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Func<CodeLensParams, Task<CodeLensContainer>> handler,
            Func<CodeLens, bool> canResolve,
            Func<CodeLens, Task<CodeLens>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link) => Task.FromException<CodeLens>(new NotImplementedException());

            return registry.AddHandler(TextDocumentNames.CodeLens,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeLensParams, CodeLensContainer,
                            CodeLensRegistrationOptions>(
                            handler,
                            registrationOptions))
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensRegistrationOptions>(
                            resolveHandler,
                            canResolve,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>, CodeLensCapability, CancellationToken> handler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                _ => new LanguageProtocolDelegatingHandlers.PartialResults<CodeLensParams, CodeLensContainer, CodeLens, CodeLensCapability,
                    CodeLensRegistrationOptions>(
                    handler,
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    x => new CodeLensContainer(x)));
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>, CodeLensCapability, CancellationToken> handler,
            Func<CodeLens, bool> canResolve,
            Func<CodeLens, CodeLensCapability, CancellationToken, Task<CodeLens>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link, cap, token) => Task.FromException<CodeLens>(new NotImplementedException());

            return
                registry.AddHandler(TextDocumentNames.CodeLens,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<CodeLensParams, CodeLensContainer, CodeLens, CodeLensCapability,
                            CodeLensRegistrationOptions>(
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new CodeLensContainer(x)))
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensCapability, CodeLensRegistrationOptions>(
                            resolveHandler,
                            canResolve,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>, CancellationToken> handler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                _ => new LanguageProtocolDelegatingHandlers.PartialResults<CodeLensParams, CodeLensContainer, CodeLens,
                    CodeLensRegistrationOptions>(
                    handler,
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    x => new CodeLensContainer(x)));
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>, CancellationToken> handler,
            Func<CodeLens, bool> canResolve,
            Func<CodeLens, CancellationToken, Task<CodeLens>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link, token) => Task.FromException<CodeLens>(new NotImplementedException());

            return registry.AddHandler(TextDocumentNames.CodeLens,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<CodeLensParams, CodeLensContainer, CodeLens,
                            CodeLensRegistrationOptions>(
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new CodeLensContainer(x)))
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensRegistrationOptions>(
                            resolveHandler,
                            canResolve,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>> handler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                _ => new LanguageProtocolDelegatingHandlers.PartialResults<CodeLensParams, CodeLensContainer, CodeLens,
                    CodeLensRegistrationOptions>(
                    handler,
                    registrationOptions,
                    _.GetRequiredService<IProgressManager>(),
                    x => new CodeLensContainer(x)));
        }

        public static ILanguageServerRegistry OnCodeLens(this ILanguageServerRegistry registry,
            Action<CodeLensParams, IObserver<IEnumerable<CodeLens>>> handler,
            Func<CodeLens, bool> canResolve,
            Func<CodeLens, Task<CodeLens>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link) => Task.FromException<CodeLens>(new NotImplementedException());

            return registry.AddHandler(TextDocumentNames.CodeLens,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<CodeLensParams, CodeLensContainer, CodeLens,
                            CodeLensRegistrationOptions>(
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new CodeLensContainer(x)))
                    .AddHandler(TextDocumentNames.CodeLensResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensRegistrationOptions>(
                            resolveHandler,
                            canResolve,
                            registrationOptions))
                ;
        }

        public static IRequestProgressObservable<IEnumerable<CodeLens>, CodeLensContainer> RequestCodeLens(
            this ITextDocumentLanguageClient mediator,
            CodeLensParams @params,
            CancellationToken cancellationToken = default)
        {
            return mediator.ProgressManager.MonitorUntil(@params, x => new CodeLensContainer(x), cancellationToken);
        }

        public static Task<CodeLens> ResolveCodeLens(this ITextDocumentLanguageClient mediator, CodeLens @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
